using System;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace celestia.network
{
	public class NetworkClient
	{
		private ClientConnection _connection;
		private Thread _receiveThread;
		private bool _isRunning;
		
		public bool IsConnected => _connection?.IsConnected ?? false;
		public float CurrentLatency => _connection?.Stats.AverageLatency ?? 0f;

		// Event handlers
		public event Action OnConnected;
		public event Action<string> OnDisconnected;
		public event Action<Packet> OnPacketReceived;

		public void Connect(string ip, int port)
		{
			try
			{
				var client = new TcpClient();
				client.Connect(ip, port);
				
				_connection = new ClientConnection(client);
				_isRunning = true;
				
				// Start receiving data in a new thread
				_receiveThread = new Thread(ReceiveLoop)
				{
					IsBackground = true
				};
				_receiveThread.Start();
				
				OnConnected?.Invoke();
				Debug.Log($"Connected to server at {ip}:{port}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to connect to server at {ip}:{port}: {e.Message}");
				Disconnect();
				throw;
			}
		}

		private void ReceiveLoop()
		{
			while (_isRunning)
			{
				try
				{
					Packet packet = _connection.ReceivePacket();
					if (packet != null)
						OnPacketReceived?.Invoke(packet);
				}
				catch (Exception e)
				{
					if (_isRunning)
					{
						Debug.LogError($"Error receiving packet: {e.Message}");
						Disconnect("Receive error");
					}
					break;
				}
			}
		}

		public void SendPacket(Packet packet)
		{
			_connection?.QueuePacket(packet);
		}

		public void Update()
		{
			_connection?.Update();
		}

		public void Disconnect(string reason = "")
		{
			if (!_isRunning) return;

			_isRunning = false;
			_connection?.Disconnect(reason);
			_connection = null;
			
			OnDisconnected?.Invoke(reason);
			Debug.Log($"Disconnected from server: {reason}");
		}

		public string GetConnectionStats()
		{
			return _connection?.Stats.ToString() ?? "Not connected";
		}
	}
}