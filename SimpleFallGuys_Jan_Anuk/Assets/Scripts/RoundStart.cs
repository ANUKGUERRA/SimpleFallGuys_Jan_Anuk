using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class RoundStart : NetworkBehaviour
{
    [SerializeField] Button startButton;
    [SerializeField] private GameManager gameManager;
    private Dictionary<ulong, bool> readyPlayers = new();

    private bool gameStarted = false;


    private void Start()
    {
        if (IsHost)
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Begin";
        }
        else
        {
            startButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ready";
        }

        startButton.onClick.AddListener(OnButtonPressed);
    }

    private void OnButtonPressed()
    {
        if (IsHost)
        {
            TryStartGame();
        }
        else
        {
            SetReadyServerRpc();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetReadyServerRpc(ServerRpcParams rpcParams = default)
    {
        ulong clientId = rpcParams.Receive.SenderClientId;

        readyPlayers[clientId] = true;

        Debug.Log($"Jugador {clientId} està Ready");
    }

    private void TryStartGame()
    {
        if (!IsServer || gameStarted)
            return;

        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (clientId == NetworkManager.Singleton.LocalClientId)
                continue;

            if (!readyPlayers.ContainsKey(clientId) || !readyPlayers[clientId])
            {
                Debug.Log("Encara no tots estan Ready");
                return;
            }
        }

        gameStarted = true;

        StartGameClientRpc();
    }

    [ClientRpc]
    private void StartGameClientRpc()
    {
        Debug.Log("La partida ha començat");

        gameManager.StartRound();
    }
}

