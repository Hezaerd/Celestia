using System;
using Riptide;
using Riptide.Utils;
using UnityEngine;

namespace celestia.network
{
	public class NetworkClient
	{
		private Client _client;

		public void Start()
		{
			RiptideLogger.Initialize(Debug.Log, Debug.Log, Debug.LogWarning, Debug.LogError, false);
			
			_client = new Client();
			
			// _client.ChangeTransport(Tcp);
			
			_client.Connected += DidConnect;
			_client.ConnectionFailed += FailedToConnect;
			_client.ClientDisconnected += PlayerLeft;
			_client.Disconnected += DidDisconnect;
		}
		
		public void Disconnect()
		{
			_client.Disconnect();
			
			_client.Connected -= DidConnect;
			_client.ConnectionFailed -= FailedToConnect;
			_client.ClientDisconnected -= PlayerLeft;
			_client.Disconnected -= DidDisconnect;
			
			_client = null;
		}

		public void OnApplicationQuit()
		{
			Disconnect();
		}
		
		public void Tick()
		{
			_client.Update();
		}
		
		public void Connect(string ip, ushort port) {
			
			_client.Connect($"{ip}:{port}");
		}
		
		private void DidConnect(object sender, EventArgs e)
		{
			throw new NotImplementedException();
		}
		
		private void FailedToConnect(object sender, ConnectionFailedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		private void PlayerLeft(object sender, ClientDisconnectedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		private void DidDisconnect(object sender, DisconnectedEventArgs e)
		{
			throw new NotImplementedException();
		}
		
		
	}
}