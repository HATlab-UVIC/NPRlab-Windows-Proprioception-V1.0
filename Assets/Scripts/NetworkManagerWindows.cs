using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using Unity.VisualScripting;
using UnityEngine;
using static NetworkDebugConsole;

public class NetworkManagerWindows : MonoBehaviour
{
    public static NetworkManagerWindows Singleton { get; private set; }
    [HideInInspector] public bool _isConnected = false;
    public event Action<ulong, ConnectionStatus> OnClientConnection;
    private ISession _activeSession;    
    private string playerNamePropertyKey = "playerName";

    ISession _ActiveSession
    {
        get => _activeSession;
        set
        {
            _activeSession = value;
            Debug.Log($"Active session: {_activeSession}");
        }
    }

    private void Awake() {
        if ( Singleton != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(NetworkManagerWindows)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        else
        {
            Singleton = this;
        }
    }

    private async void Start()
    {
        try
        {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            Debug.Log($"Sign in anonymously successed! Player ID: {AuthenticationService.Instance.PlayerId}");

            StartSessionAsHost();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
        if (NetworkManager.Singleton == null)
        {
            throw new Exception($"There is no {nameof(NetworkManager)} for the {nameof(NetworkDebugConsole)} to do stuff with! " +
                $"Please add a {nameof(NetworkManager)} to the scene.");
        }
        NetworkManager.Singleton.OnClientConnectedCallback += OnNetworkConnectionEvent;
        NetworkManager.Singleton.OnClientDisconnectCallback += OnNetworkDisconnectionEvent;
    }

    private void OnDestroy() {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnNetworkConnectionEvent;
            NetworkManager.Singleton.OnClientDisconnectCallback -= OnNetworkDisconnectionEvent;
        }
    }

    private void OnNetworkConnectionEvent(ulong clientId) {
        OnClientConnection?.Invoke(clientId, ConnectionStatus.Connected);
        if (NetworkManager.Singleton.LocalClientId == clientId && NetworkManager.Singleton.IsHost)
        {
            NetworkDebugConsole.Singleton.SetDebugString("Hosted with id: " + clientId);
        }
        else if (NetworkManager.Singleton.ConnectedClientsIds.Contains(clientId))
        {
            NetworkDebugConsole.Singleton.SetDebugString("Client connected with id: " + clientId);
            _isConnected = true;
        }
    }
    private void OnNetworkDisconnectionEvent(ulong clientId) {
        OnClientConnection?.Invoke(clientId, ConnectionStatus.Disconnected);
        try
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            // await LobbyService.Instance.RemovePlayerAsync(_joinCode.text, clientId);
        }
        catch (Exception e)
        {
            NetworkDebugConsole.Singleton.SetDebugString($"Error: {e.Message}");
        }
        NetworkDebugConsole.Singleton.SetDebugString("Client disconnected with id: " + clientId);
        ControlManager.Singleton.ResetTest();
        _isConnected = false;
    }

    private async Task<Dictionary<string, PlayerProperty>> GetPlayerProperties() {
        var playerName = await AuthenticationService.Instance.GetPlayerNameAsync();
        var playerNameProperty = new PlayerProperty(playerName, VisibilityPropertyOptions.Member);
        return new Dictionary<string, PlayerProperty> { { playerNamePropertyKey,  playerNameProperty } };
    }

    private async void StartSessionAsHost() {
        var playerProperties = await GetPlayerProperties();

        var options = new SessionOptions
        {
            MaxPlayers = 2,
            IsLocked = false,
            IsPrivate = false,
            PlayerProperties = playerProperties,
        }.WithRelayNetwork();

        _ActiveSession = await MultiplayerService.Instance.CreateSessionAsync(options);
        Debug.Log($"Session {_ActiveSession.Id} created! Join code: {_ActiveSession.Code}");
        NetworkDebugConsole.Singleton.SetDebugString($"Session {NetworkManager.Singleton.LocalClientId} created! Join code: {_ActiveSession.Code}");
        NetworkDebugConsole.Singleton.SetJoingCode(_ActiveSession.Code);
    }

    private async Task LeaveSession() {
        if ( _ActiveSession != null )
        {
            try
            {
                await _ActiveSession.LeaveAsync();
            }
            catch
            {
                // Ignored as we are exiting the game
            }
            finally
            {
                _ActiveSession = null;
            }
        }
    }
}
