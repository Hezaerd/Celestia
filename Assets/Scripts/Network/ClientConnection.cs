using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using celestia.network.packets;
using UnityEngine;

namespace celestia.network
{
	public class ClientConnection
	{
		#region Properties & Fields
		public int Id { get; private set; }
		public bool IsConnected { get; private set; }
		public ConnectionStats Stats { get; private set; }
		
		// TCP client and stream
		private readonly TcpClient _client;
		private readonly NetworkStream _stream;
		private readonly Queue<Packet> _packetQueue;
		private readonly object _lockObject;
		
		// Buffer size for reading packets
		private readonly byte[] _receiveBuffer;
		private const int BUFFER_SIZE = 4096;

		// Client ID generation
		private static int _lastClientId = 0;
		
		// Heartbeat
		private DateTime _lastHeartbeatReceived;
		private DateTime _lastHeartbeatSent;
		private const float HEARTBEAT_TIMEOUT = 10f; // seconds - Time before disconnecting
		private const float HEARTBEAT_INTERVAL = 1f; // seconds - Time between heartbeats
		private float _currentLatency;
		#endregion
		
		#region Connection Stats Class
		public class ConnectionStats
		{
			public long BytesSent { get; private set; }
			public long BytesReceived { get; private set; }
			public int PacketsSent { get; private set; }
			public int PacketsReceived { get; private set; }
			public float AverageLatency { get; private set; }
			public float PacketLoss { get; private set; }
			public DateTime ConnectedSince { get; }

			private readonly Queue<float> _latencyHistory;
			private const int LATENCY_HISTORY_SIZE = 10;
			
			private int _totalPacketsSent;
			private int _totalPacketsAcked;

			public ConnectionStats()
			{
				_latencyHistory = new Queue<float>();
				ConnectedSince = DateTime.Now;
			}

			public void AddLatencySample(float latency)
			{
				_latencyHistory.Enqueue(latency);
				if (_latencyHistory.Count > LATENCY_HISTORY_SIZE)
					_latencyHistory.Dequeue();

				AverageLatency = _latencyHistory.Average();
			}

			public void TrackSentData(int bytes)
			{
				BytesSent += bytes;
				PacketsSent++;
				_totalPacketsSent++;
				UpdatePacketLoss();
			}

			public void TrackReceiveData(int bytes)
			{
				BytesReceived += bytes;
				PacketsReceived++;
				_totalPacketsAcked++;
				UpdatePacketLoss();
			}

			private void UpdatePacketLoss()
			{
				if (_totalPacketsSent > 0)
					PacketLoss = 1f - (_totalPacketsAcked / (float)_totalPacketsSent);
			}

			public string GetStateString()
			{
				return $"Connected: {(DateTime.Now - ConnectedSince):hh\\:mm\\:ss}\n" +
					   $"Sent: {BytesSent / 1024f:F2} KB ({PacketsSent} packets)\n" +
					   $"Received: {BytesReceived / 1024f:F2} KB ({PacketsReceived} packets)\n" +
					   $"Latency: {AverageLatency:F1}ms\n" +
					   $"Packet Loss: {PacketLoss * 100:F1}%";
			}
		}
		#endregion
		
		public ClientConnection(TcpClient client)
		{
			Id = GenerateClientId();
			_client = client;
			_stream = _client.GetStream();
			IsConnected = true;

			_packetQueue = new Queue<Packet>();
			_lockObject = new object();
			_receiveBuffer = new byte[BUFFER_SIZE];
			
			Stats = new ConnectionStats();
			
			// Initialize heartbeat timestamps
			_lastHeartbeatReceived = DateTime.Now;
			_lastHeartbeatSent = DateTime.Now;
			
			// Configure TCP settings
			_client.NoDelay = true; // Disable Nagle's algorithm https://en.wikipedia.org/wiki/Nagle%27s_algorithm
			_client.ReceiveBufferSize = BUFFER_SIZE;
			_client.SendBufferSize = BUFFER_SIZE;
		}

		private static int GenerateClientId()
		{
			return Interlocked.Increment(ref _lastClientId);
		}

		#region Packet Queue
		public void QueuePacket(Packet packet)
		{
			if (!IsConnected) return;
			
			lock(_lockObject)
				_packetQueue.Enqueue(packet);
		}

		public void ProcessPacketQueue()
		{
			if (!IsConnected) return;

			lock (_lockObject)
			{
				while (_packetQueue.Any())
				{
					Packet packet = _packetQueue.Dequeue();
					SendPacketInternal(packet);
				}
			}
		}
		#endregion

		#region Packet Sending & Receiving
		private void SendPacketInternal(Packet packet)
		{
			try
			{
				// Create packet data
				NetworkWriter writer = new NetworkWriter();
				writer.WriteInt((int)packet.Type);
				packet.Serialize(ref writer);
				
				var data = writer.ToArray();
				var lengthPrefix = BitConverter.GetBytes(data.Length);
				
				// Send length prefix and data
				_stream.Write(lengthPrefix, 0, lengthPrefix.Length);
				_stream.Write(data, 0, data.Length);
				_stream.Flush();
				
				// Update stats
				Stats.TrackSentData(lengthPrefix.Length + data.Length);
			}
			catch (Exception e)
			{
				Debug.LogError($"Error sending packet to client {Id}: {e.Message}");
				Disconnect("Send error");
			}
		}

		public Packet ReceivePacket()
		{
			if (!IsConnected) return null;

			try
			{
				// Read packet size
				var lengthBuffer = new byte[4];
				var bytesRead = _stream.Read(lengthBuffer, 0, lengthBuffer.Length);

				if (bytesRead < 4)
					throw new Exception("Failed to read packet length");

				var packetSize = BitConverter.ToInt32(lengthBuffer, 0);
				
				if (packetSize is <= 0 or > BUFFER_SIZE)
					throw new Exception($"Invalid packet size: {packetSize}");
				
				// Read packet data
				var packetData = new byte[BUFFER_SIZE];
				var totalBytesRead = 0;

				while (totalBytesRead < packetSize)
				{
					var remainingBytes = packetSize - totalBytesRead;
					var bytesReceived = _stream.Read(packetData, totalBytesRead, remainingBytes);
					
					if (bytesReceived == 0)
						throw new Exception("Connection closed by remote host");
					
					totalBytesRead += bytesReceived;
				}
				
				// Update stats
				Stats.TrackReceiveData(totalBytesRead + 4); // +4 for length prefix
				
				// Read packet type
				NetworkReader reader = new NetworkReader(packetData);
				Packet.PacketType packetType = (Packet.PacketType)reader.ReadInt();
				
				Packet packet = CreatePacket(packetType);
				packet.Deserialize(ref reader);

				// Handle hearthbeat packet
				if (packet is HeartbeatPacket heartbeat)
				{
					HandleHeartbeat(heartbeat);
					return null; // Don't propagate heartbeat packets
				}
				
				return packet;
			}
			catch (Exception e)
			{
				Debug.LogError($"Error receiving packet from client {Id}: {e.Message}");
				Disconnect("Receive error");
				return null;
			}
		}
		
		private static Packet CreatePacket(Packet.PacketType type)
		{
			return type switch
			{
				Packet.PacketType.HEARTBEAT => new HeartbeatPacket(),
				_ => throw new Exception($"Unknown packet type: {type}")
			};
		}
		#endregion
		
		#region Heartbeat System
		private void HandleHeartbeat(HeartbeatPacket heartbeat)
		{
			_lastHeartbeatReceived = DateTime.Now;

			if (heartbeat.IsResponse)
			{
				_currentLatency = (float)(DateTime.Now - _lastHeartbeatSent).TotalMilliseconds;
				Stats.AddLatencySample(_currentLatency);
			}
			else
			{
				// Send heartbeat response
				var response = new HeartbeatPacket { IsResponse = true };
				QueuePacket(response);
			}
		}

		private void UpdateHeartbeat()
		{
			var now = DateTime.Now;
			
			// Check if we need to send a heartbeat
			if ((now - _lastHeartbeatSent).TotalSeconds >= HEARTBEAT_INTERVAL)
			{
				QueuePacket(new HeartbeatPacket() { IsResponse = false });
				_lastHeartbeatSent = now;
			}
			
			// Check for timeout
			if ((now - _lastHeartbeatReceived).TotalSeconds >= HEARTBEAT_TIMEOUT)
			{
				Debug.LogWarning($"Client {Id} timed out (no heartbeat for {HEARTBEAT_TIMEOUT} seconds)");
				Disconnect("Heartbeat timeout");
			}
		}
		#endregion

		public void Update()
		{
			if (!IsConnected) return;
			
			UpdateHeartbeat();
			ProcessPacketQueue();
		}
		
		public void Disconnect(string reason = "")
		{
			if (!IsConnected) return;

			IsConnected = false;

			try
			{
				_stream?.Close();
				_client?.Close();
			}
			catch (Exception e)
			{
				Debug.Log($"Error disconnecting client {Id}: {e.Message}");
			}
			
			// Notify server
			Debug.Log($"Client {Id} disconnected: {reason}");
			NetworkManager.Instance.OnClientDisconnected(this);
		}
	}
}