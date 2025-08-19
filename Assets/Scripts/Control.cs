using Unity.Collections;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Multiplayer;
using UnityEditor;
using UnityEngine;

public class Control : MonoBehaviour
{
    [SerializeField] private GameObject _prefab01;

    void Start() {
        NetworkManager.Singleton.OnServerStarted += () =>
        {
            NetworkManager.Singleton.CustomMessagingManager.RegisterNamedMessageHandler(
                "HelloFromClient", OnHelloMessageReceived);
        };
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SpawnByNumber(1);
        }
    }

    public void SpawnByNumber(int number) {
        switch (number)
        {
            case 1:
                var instance = Instantiate(_prefab01);
                NetworkDebugConsole.Singleton.SetDebugString("Prefab 01 instantiated locally");
                SpawnCutomMessage(1);
                break;
            default:
                NetworkDebugConsole.Singleton.SetDebugString("Not a valid number input");
                break;
        }
    }

    public void SpawnCutomMessage(int number) {
        using var writer = new FastBufferWriter(128, Allocator.Temp);
        writer.WriteValueSafe(999);
        writer.WriteValueSafe(new FixedString64Bytes("Hi clients!"));

        NetworkManager.Singleton.CustomMessagingManager.SendNamedMessage(
            "HelloFromServer",
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
}
