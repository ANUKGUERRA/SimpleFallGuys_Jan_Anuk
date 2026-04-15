using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine;

public class NetworkUI : MonoBehaviour
{
    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += (ulong nosequees) =>
        {
            Debug.Log(nosequees);
        };
        
    }
    string ip = "";
    private void OnGUI()
    {
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
        }

        ip = GUI.TextField(new Rect(10, 10, 200, 30), ip);

        if (GUI.Button(new Rect(10, 50, 100, 30), "Host"))
        {
            NetworkManager.Singleton.StartHost();
        }

        if (GUI.Button(new Rect(10, 100, 100, 30), "Client"))
        {
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = ip;
            NetworkManager.Singleton.StartClient();
        }
    }
}
