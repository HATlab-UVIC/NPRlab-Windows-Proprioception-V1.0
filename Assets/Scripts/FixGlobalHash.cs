using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public static class FixGlobalHash
{
#if UNITY_EDITOR
    [MenuItem("Tools/Fix NetworkObjects in Scene")]
    public static void FixNetworkObjectsInScene() {
        // var networkObjects = Object.FindObjectsOfType<NetworkObject>(true);
        var networkObject = Object.FindAnyObjectByType<ControlManager>();
        if (!networkObject.gameObject.scene.isLoaded) return;

        Debug.Log($"Object found. Name {networkObject.name}");
        var serializedObject = new SerializedObject(networkObject);
        var hashField = serializedObject.FindProperty("GlobalObjectIdHash");

        hashField.uintValue = 0;
        serializedObject.ApplyModifiedProperties();
    }
#endif
}
