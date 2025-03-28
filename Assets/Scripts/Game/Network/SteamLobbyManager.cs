using Netcode.Transports.Facepunch;
using Sirenix.OdinInspector;
using Steamworks;
using Steamworks.Data;
using Unity.Netcode;
using UnityEngine;

namespace game.network
{
    public class SteamLobbyManager : MonoBehaviour
    {
        [Title("Steam Lobby Manager", TitleAlignment = TitleAlignments.Centered)]
        
        [SerializeField] private bool debugLog;
        
        private NetworkManager _networkManager;
        private FacepunchTransport _transport;
        private Lobby? _lobby;
        
        private void Awake()
        {
            _networkManager = NetworkManager.Singleton;
            _transport = _networkManager.GetComponent<FacepunchTransport>();

            SteamMatchmaking.OnLobbyCreated += OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered += OnSteamLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnSteamLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave += OnSteamLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite += OnSteamLobbyInvite;
            
            SteamFriends.OnGameLobbyJoinRequested += OnSteamGameLobbyJoinRequested;
        }

        private void OnDestroy()
        {
            SteamMatchmaking.OnLobbyCreated -= OnLobbyCreated;
            SteamMatchmaking.OnLobbyEntered -= OnSteamLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined -= OnSteamLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberLeave -= OnSteamLobbyMemberLeave;
            SteamMatchmaking.OnLobbyInvite -= OnSteamLobbyInvite;
            
            SteamFriends.OnGameLobbyJoinRequested -= OnSteamGameLobbyJoinRequested;
        }
        
        private void OnApplicationQuit()
        {
            Disconnect();
        }

        public async void StartHost(uint maxPlayers)
        {
            _networkManager.OnClientConnectedCallback += OnClientConnectedCallback;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnectedCallback;
            _networkManager.OnServerStarted += OnServerStarted;

            _networkManager.StartHost();

            _lobby = await SteamMatchmaking.CreateLobbyAsync((int)maxPlayers);
        }

        public void StartClient(SteamId ownerId)
        {
            _networkManager.OnClientConnectedCallback += OnClientConnected;
            _networkManager.OnClientDisconnectCallback += OnClientDisconnected;
            
            _transport.targetSteamId = ownerId;
            
            if (_networkManager.StartClient())
                Debug.Log("Client started");
            else
                Debug.LogError("Failed to start client");
        }

        public void Disconnect()
        {
            _lobby?.Leave();
            _networkManager.Shutdown();
        }

        #region Steam Callbacks

        private void OnSteamGameLobbyJoinRequested(Lobby lobby, SteamId steamId)
        {
            var isSame = lobby.Owner.Id == steamId;

            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Game lobby join requested by {steamId}, owner: {lobby.Owner.Id}, is same: {isSame}");
            
            StartClient(steamId);
        }

        private void OnSteamLobbyInvite(Friend friend, Lobby lobby)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Lobby invite from {friend.Id}, lobby: {lobby.Id}");
        }
        
        private void OnSteamLobbyMemberLeave(Lobby lobby, Friend friend) { }

        private void OnSteamLobbyMemberJoined(Lobby lobby, Friend friend) { }
        
        private void OnSteamLobbyEntered(Lobby lobby)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Lobby entered: {lobby.Id}");

            if (_networkManager.IsHost)
                return;
            
            StartClient(lobby.Owner.Id);
        }
        
        private void OnLobbyCreated(Result result, Lobby lobby)
        {
            if (result != Result.OK)
            {
                Debug.LogError($"[SteamLobbyManager] Failed to create lobby: {result}");
                return;
            }
            
            // Apply lobby default settings
            lobby.SetFriendsOnly();
            lobby.SetData("name", $"{SteamClient.Name}'s Lobby");
            lobby.SetJoinable(true);

            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Lobby created: {lobby.GetData("name")}/{lobby.Id}");
        }
        
        #endregion
        
        #region Network Callbacks
        
        private void OnClientConnectedCallback(ulong clientId)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Client connected: {clientId}");
        }
        
        private void OnClientDisconnectedCallback(ulong clientId)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] Client disconnected: {clientId}");
        }

        private void OnServerStarted() { }
        
        private void OnClientConnected(ulong clientId)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] I'm connected: {clientId}");
        }
        
        private void OnClientDisconnected(ulong clientId)
        {
            if (debugLog)
                Debug.Log($"[SteamLobbyManager] I'm disconnected: {clientId}");
            
            _networkManager.OnClientConnectedCallback -= OnClientConnected;
            _networkManager.OnClientDisconnectCallback -= OnClientDisconnected;
        }
        
        #endregion
    }
}
