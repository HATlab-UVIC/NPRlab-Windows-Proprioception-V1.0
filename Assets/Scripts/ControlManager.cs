using System;
using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEditor;
using UnityEngine;

public class ControlManager : MonoBehaviour
{
    public static ControlManager Singleton { get; private set; }
    [SerializeField] private GameObject _prefabTarget;
    [SerializeField] private float _pivotDistance = 0.2f;
    [SerializeField] private float _pivotScale = 0.2f;
    private GameObject[] _targetArray = new GameObject[9];
    private Vector3 _pivotPosition;

    void Awake() {
        if (Singleton != null)
        {
            throw new Exception($"Detected more than one instance of {nameof(NetworkDebugConsole)}! " +
                $"Do you have more than one component attached to a {nameof(GameObject)}");
        }
        Singleton = this;
    }

    void Start() {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                "HelloFromClient", OnHelloMessageReceived);
        };
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                "CaptureFromClient", OnCaptureMessageReceived);
        };
        _pivotPosition = new Vector3(-_pivotScale, _pivotScale, 0);
    }

    void Update() {
        for (int i = 1; i <= 9; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                if (_targetArray[i- 1] == null)
                {
                    SpawnByNumber(i);
                    break;
                }
                else {
                    NetworkDebugConsole.Singleton.SetDebugString($"Target {i} already exist!");
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            ResetTest();
        }
    }

    private void ResetTest() {
        for (int i = 0; i < 9; i++)
        {
            DespawnCustomMessage(i + 1);
            GameObject.Destroy(_targetArray[i]);
            _targetArray[i] = null;
        }
    }

    public void SpawnByNumber(int number) {
        GameObject instance;
        number -= 1;
        switch (number)
        {
            case 0:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 1:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 2:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 3:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 4:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 5:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 6:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 7:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            case 8:
                instance = Instantiate(_prefabTarget);
                instance.transform.position = _pivotPosition + new Vector3((number % 3) * _pivotScale, -(number / 3) * _pivotScale, _pivotDistance);
                NetworkDebugConsole.Singleton.SetDebugString($"Prefab {number} instantiated locally");
                _targetArray[number] = instance;
                number += 1;
                SpawnCustomMessage(number);
                break;
            default:
                NetworkDebugConsole.Singleton.SetDebugString("Not a valid number input");
                break;
        }
    }

    public void SpawnCustomMessage(int number) {
        using var writer = new FastBufferWriter(128, Allocator.Temp);
        writer.WriteValueSafe(number);
        writer.WriteValueSafe(new FixedString64Bytes("Spawn"));

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "SpawnFromServer",
            NetworkManager.Singleton.ConnectedClientsIds,
            writer
        );
    }

    public void DespawnCustomMessage(int number) {
        using var writer = new FastBufferWriter(128, Allocator.Temp);
        writer.WriteValueSafe(number);
        writer.WriteValueSafe(new FixedString64Bytes("Despawn"));

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "DespawnFromServer",
            NetworkManager.Singleton.ConnectedClientsIds,
            writer
        );
    }

    private void OnHelloMessageReceived(ulong senderClientId, FastBufferReader reader) {
        // Read data in the same order it was written
        reader.ReadValueSafe(out int number);
        reader.ReadValueSafe(out FixedString64Bytes text);

        Debug.Log($"[Server] Received from {senderClientId}: {number}, {text}");
        NetworkDebugConsole.Singleton.SetDebugString($"Received from {senderClientId}: {number}, {text}");
    }

    private void OnCaptureMessageReceived(ulong senderClientId, FastBufferReader reader) {
        // Read data in the same order it was written
        reader.ReadValueSafe(out int number);
        reader.ReadValueSafe(out FixedString64Bytes text);

        Debug.Log($"[Server] Received from {senderClientId}: {number}, {text}");
        NetworkDebugConsole.Singleton.SetDebugString($"Received from {senderClientId}: {number}, {text}");
        GameObject.Destroy(_targetArray[number - 1]);
        _targetArray[number - 1] = null;
    }
}
