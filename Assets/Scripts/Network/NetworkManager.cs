using UnityEngine;

namespace celestia.network
{
	public class NetworkManager : MonoBehaviour
	{
		public static NetworkManager Instance { get; private set; }
		
		[Header("Network Settings")]
		[SerializeField] private bool showDebugUI = true;
		[SerializeField] private ushort defaultPort = 7777;
		[SerializeField] private ushort maxConnections = 4;
		
		private NetworkServer _server;
		private NetworkClient _client;

		public bool IsServer => _server != null;
		public bool IsClient => _client != null;

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
		
		private void FixedUpdate()
		{
			_server?.Tick();
			_client?.Tick();
		}

		private void OnApplicationQuit()
		{
			_server?.OnApplicationQuit();
			_client?.OnApplicationQuit();
		}

		public void StartServer()
		{
			_server = new NetworkServer();
			_server.Start(defaultPort, maxConnections);
		}
		
		public void StartClient(string ip)
		{
			_client = new NetworkClient();
			_client.Start();
			
			_client.Connect(ip, defaultPort);
		}
	}
}