using System;
using Riptide;
using Riptide.Utils;
using UnityEngine;

namespace celestia.network
{
	public class NetworkServer
	{
		private Server _server;

		public event Action OnClientConnected;
		public event Action OnClientDisconnected;
		
		public void Start(ushort port, ushort maxConnections)
		{
			_server = new Server();
			
			QualitySettings.vSyncCount = 0;
			Application.targetFrameRate = 30;
			
#if UNITY_EDITOR
			RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
#else
			Console.Title = "Celestia Dedicated Server";
			Console.Clear();
			Application.SetStackTraceLogType(UnityEngine.LogType.Log, StackTraceLogType.None);
			RiptideLogger.Initialize(Debug.Log, true);
#endif

			_server.ClientConnected += NewPlayerConnected;
			_server.ClientDisconnected += PlayerLeft;
			
			_server.Start(port, maxConnections);
		}

		public void Stop()
		{
			_server.Stop();
			
			_server.ClientConnected -= NewPlayerConnected;
			_server.ClientDisconnected -= PlayerLeft;
			
			_server = null;
		}

		public void Tick()
		{
			_server.Update();
		}

		public void NewPlayerConnected(object sender, ServerConnectedEventArgs e)
		{
			
		}

		public void PlayerLeft(object sender, ServerDisconnectedEventArgs e)
		{
			
		}

		public void OnApplicationQuit()
		{
			Stop();
		}
	}
}