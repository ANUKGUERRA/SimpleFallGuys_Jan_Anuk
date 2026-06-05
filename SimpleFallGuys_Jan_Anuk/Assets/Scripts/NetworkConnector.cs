using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkConnector : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "";
    private void Start()
    {
        NetworkManager.Singleton.OnServerStarted += OnServerStarted;
    }

    private void OnServerStarted()
    {
        NetworkManager.Singleton.SceneManager.LoadScene(gameSceneName, LoadSceneMode.Single);
    }

    private void OnDisable()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnServerStarted -= OnServerStarted;
    }
}