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
