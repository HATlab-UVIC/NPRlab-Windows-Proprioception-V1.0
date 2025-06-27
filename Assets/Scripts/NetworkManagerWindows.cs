using Unity.Netcode;
using UnityEngine;

public class NetworkManagerWindows : MonoBehaviour
{
    private void Start()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void OnDestroy() {
    }
}
