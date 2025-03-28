using System;
using UnityEngine;

namespace celestia.network
{
	public class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Instance { get; private set; }

		[Header("Network Settings")]
		[SerializeField] private string defaultIp = "127.0.0.1";
		[SerializeField] private int defaultPort = 7777;
		[SerializeField] private bool showDebugUI = true;
		
		private NetworkServer _server;
		private NetworkClient _client;
		private readonly object _lock = new object();
		
		public bool IsServer => _server != null;
		public bool IsClient => _client != null;
		public bool IsConnected => IsClient && _client.IsConnected;
		
		#region Unity Methods
		private void Awake()
		{
			if (Instance == null)
			{
				Instance = this;
				DontDestroyOnLoad(this);
			}
			else
			{
				Destroy(gameObject);
			}
		}

		private void Update()
		{
			// Update server if running
			_server?.Update();
			
			// Update client if running
			_client?.Update();
		}

		private void OnGUI()
		{
			if (!showDebugUI || !Debug.isDebugBuild) return;
			
			GUILayout.BeginArea(new Rect(10, 10, 300, Screen.height - 20));

			if (!IsServer && !IsClient)
			{
				if (GUILayout.Button("Start Server"))
					StartServer();
				
				if (GUILayout.Button("Start Client"))
					StartClient();
			}
			else
			{
				string status = IsServer ? "Server" : "Client";
				GUILayout.Label($"Running as: {status}");

				if (IsServer)
				{
					GUILayout.Label($"Connected Clients: {_server.ClientCount}");
				}

				if (IsClient)
				{
					GUILayout.Label($"Connection Status: {(_client.IsConnected ? "Connected" : "Disconnected")}");
					if (_client.IsConnected)
						GUILayout.Label($"Latency: {_client.CurrentLatency:F1}ms");
				}
				
				if (GUILayout.Button("Stop"))
					Stop();
			}
			
			GUILayout.EndArea();
		}
		#endregion

		#region Network Control
		public void StartServer()
		{
			if (IsServer || IsClient) return;

			try
			{
				_server = new NetworkServer();
				_server.Start(defaultPort);
				Debug.Log($"Server started on port {defaultPort}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to start server: {e.Message}");
				_server = null;
			}
		}

		public void StartClient()
		{
			if (IsServer || IsClient) return;

			try
			{
				_client = new NetworkClient();
				_client.Connect(defaultIp, defaultPort);
				Debug.Log($"Connected to server {defaultIp}:{defaultPort}");
			}
			catch (Exception e)
			{
				Debug.LogError($"Failed to connect to server: {e.Message}");
				_client = null;
			}
		}

		public void Stop()
		{
			_server?.Stop();
			_server = null;

			_client?.Disconnect();
			_client = null;
			
			Debug.Log("Network stopped");
		}
		#endregion
		
		#region Server Methods
		public void OnClientDisconnected(ClientConnection client)
		{
			_server?.OnClientDisconnected(client);
		}

		public void BroadcastPacket(Packet packet, int? excludeClientId = null)
		{
			_server?.BroadcastPacket(packet, excludeClientId);
		}

		public void SendToClient(int clientId, Packet packet)
		{
			_server?.SendToClient(clientId, packet);
		}
		#endregion
		
		#region Client Methods
		public void SendToServer(Packet packet)
		{
			_client?.SendPacket(packet);
		}
		#endregion
	}
}