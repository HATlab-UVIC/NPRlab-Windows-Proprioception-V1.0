using System;
using System.Linq;
using System.Net.Security;
using System.Threading.Tasks;
using TMPro;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEngine;
using UnityEngine.Windows;

public class NetworkDebugConsole : MonoBehaviour
{
    public static NetworkDebugConsole Singleton { get; private set; }
    public enum ConnectionStatus
    {
        Connected,
        Disconnected,
    }
    public event Action<ulong, ConnectionStatus> OnClientConnection;
    [SerializeField] private TMP_Text _debugConsoet;
    [SerializeField] private TMP_Text _joinCode;
    private int _lineCount = 0;

    private void Awake() {
        if (Singleton != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(NetworkDebugConsole)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

    private void Start() {
        if (Singleton != this)
        {
            return;
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
            SetDebugString("Hosted with id: " + clientId);
        }
        else if (NetworkManager.Singleton.ConnectedClientsIds.Contains(clientId)) {
            SetDebugString("Client connected with id: " + clientId);
        }
    }
    private async void OnNetworkDisconnectionEvent(ulong clientId) {
        OnClientConnection?.Invoke(clientId, ConnectionStatus.Disconnected);
        try
        {
            NetworkManager.Singleton.DisconnectClient(clientId);
            // await LobbyService.Instance.RemovePlayerAsync(_joinCode.text, clientId);
        }
        catch (Exception e)
        {
            SetDebugString($"Error: {e.Message}");
        }
        SetDebugString("Client disconnected with id: " + clientId);
    }

    public void SetDebugString(string str) {    
        string[] lines = _debugConsoet.text.Split(new[] { '\n' }, StringSplitOptions.None);
        if (_lineCount >= 2)
        {
            int index = _debugConsoet.text.IndexOf(System.Environment.NewLine);
            _debugConsoet.text = string.Join("\n", lines.Skip(1)); ;
            _lineCount--;
        }
        _debugConsoet.text += DateTime.Now.ToString("HH:mm:ss") + " " + str + "\n";
        _lineCount++;
        Debug.Log(_debugConsoet.text);
    }

    public void SetJoingCode(string joingCode) {
        _joinCode.text = $"Join Code: {joingCode}";
    }
}
