using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    string ip = "127.0.0.1";

    private void Start()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong clientId) =>
        {
            Debug.Log("Client connected: " + clientId);
        };
    }

    private void OnGUI()
    {
        if (NetworkManager.Singleton.IsClient || NetworkManager.Singleton.IsServer)
            return;

        ip = GUI.TextField(new Rect(10, 10, 200, 30), ip);

        if (GUI.Button(new Rect(10, 50, 100, 30), "Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUI.Button(new Rect(10, 100, 100, 30), "Client"))
        {
            ConnectClient();
        }
    }

    void ConnectClient()
    {
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

        transport.SetConnectionData(ip, 7777);

        NetworkManager.Singleton.StartClient();
    }
}