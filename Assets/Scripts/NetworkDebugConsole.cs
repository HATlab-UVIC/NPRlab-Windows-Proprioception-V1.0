using System;
using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEditor.PackageManager;
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
    [SerializeField] private TMP_Text _tmpText;
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
    private void OnNetworkDisconnectionEvent(ulong clientId) {
        OnClientConnection?.Invoke(clientId, ConnectionStatus.Disconnected);
        SetDebugString("Client disconnected with id: " + clientId);
    }

    private void SetDebugString(string str) {
        string[] lines = _tmpText.text.Split(new[] { '\n' }, StringSplitOptions.None);
        if (_lineCount >= 2)
        {
            int index = _tmpText.text.IndexOf(System.Environment.NewLine);
            _tmpText.text = string.Join("\n", lines.Skip(1)); ;
            _lineCount--;
        }
        _tmpText.text += DateTime.Now.ToString("HH:mm:ss") + " " + str + "\n";
        _lineCount++;
        Debug.Log(_tmpText.text);
    }

}
