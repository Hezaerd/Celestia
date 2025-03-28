using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace celestia.network
{
	public class NetworkServer
	{
		private TcpListener _listener;
		private readonly Dictionary<int, ClientConnection> _clients = new Dictionary<int, ClientConnection>();
		private readonly object _clientsLock = new object();
		private bool _isRunning;
		private Thread _acceptThread;

		public int ClientCount 
		{
			get
			{
				lock(_clientsLock)
					return _clients.Count;
			}
		}
		
		public void Start(int port)
		{
			try
			{
				_listener = new TcpListener(IPAddress.Any, port);
				_listener.Start();
				_isRunning = true;

				// Accept clients in a separate thread
				_acceptThread = new Thread(AcceptClients)
				{
					IsBackground = true
				};
				_acceptThread.Start();
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to start server: {e.Message}");
				throw;
			}
		}

		private void AcceptClients()
		{
			while (_isRunning)
			{
				try
				{
					TcpClient tcpClient = _listener.AcceptTcpClient();
					HandleNewClient(tcpClient);
				}
				catch (Exception e)
				{
					if (_isRunning)
						Debug.LogError($"Error accepting client: {e.Message}");
				}
			}
		}

		private void HandleNewClient(TcpClient tcpClient)
		{
			var client = new ClientConnection(tcpClient);

			lock(_clientsLock)
			{
				_clients.Add(client.Id, client);
			}
			
			// Start a new thread to handle the client
			Thread clientThread = new Thread(() => HandleClientMessages(client))
			{
				IsBackground = true
			};
			clientThread.Start();
			
			Debug.Log($"Client {client.Id} connected from {tcpClient.Client.RemoteEndPoint}");
		}

		private void HandleClientMessages(ClientConnection client)
		{
			while (client.IsConnected)
			{
				try
				{
					Packet packet = client.ReceivePacket();
					if (packet != null)
						ProcessPacket(client, packet);
				}
				catch (Exception e)
				{
					if (client.IsConnected)
					{
						Debug.LogError($"Error handling client {client.Id}: {e.Message}");
						client.Disconnect("Error processing messages");
					}
					break;
				}
			}
		}

		private void ProcessPacket(ClientConnection client, Packet packet)
		{
			switch (packet.Type)
			{
				default:
					Debug.LogWarning($"Unhandled packet type: {packet.Type}");
					break;
			}
		}

		public void Update()
		{
			if (!_isRunning) return;

			lock(_clientsLock)
			{
				foreach (var client in _clients.Values)
				{
					client.Update();
				}
			}
		}

		public void BroadcastPacket(Packet packet, int? excludeClientId = null)
		{
			lock(_clientsLock)
			{
				foreach (var client in _clients.Values)
				{
					if (excludeClientId.HasValue && client.Id == excludeClientId.Value)
						continue;
					
					client.QueuePacket(packet);
				}
			}
		}

		public void SendToClient(int clientId, Packet packet)
		{
			lock(_clientsLock)
			{
				if (_clients.TryGetValue(clientId, out var client))
					client.QueuePacket(packet);
			}
		}

		public void OnClientDisconnected(ClientConnection client)
		{
			lock(_clientsLock)
			{
				if (_clients.ContainsKey(client.Id))
				{
					_clients.Remove(client.Id);
					Debug.Log($"Client {client.Id} disconnected");
				}
			}
		}

		public void Stop()
		{
			_isRunning = false;
			
			// Disconnect all clients
			lock(_clientsLock)
			{
				foreach (ClientConnection client in _clients.Values)
					client.Disconnect("Server stopping");
				_clients.Clear();
			}
			
			// Stop listening for new connections
			_listener?.Stop();
			
			// Wait for the accept thread to finish
			_acceptThread?.Join(1000);
		}
	}
}