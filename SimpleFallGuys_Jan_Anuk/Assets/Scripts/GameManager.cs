using System.Linq;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private PlayerData[] players;

    [SerializeField] private TMP_Text[] leaderboardTexts;
    [SerializeField] private TMP_Text roundText;

    private int currentRound = 1;

    private void Start()
    {
        players = FindObjectsByType<PlayerData>(
            FindObjectsSortMode.None);

        foreach (var player in players)
        {
            player.Points.OnValueChanged += (_, __) => MostrarRanking();
        }

        MostrarRanking();
        UpdateRoundText();
    }

    public void MostrarRanking()
    {
        var topPlayers = players
            .OrderByDescending(p => p.Points.Value)
            .Take(4)
            .ToArray();

        for (int i = 0; i < leaderboardTexts.Length; i++)
        {
            if (i < topPlayers.Length)
            {
                leaderboardTexts[i].text =
                    $"{topPlayers[i].Nickname.Value}: {topPlayers[i].Points.Value}";
            }
            else
            {
                leaderboardTexts[i].text = "-";
            }
        }
    }

    public void PlayerFinished(PlayerData player)
    {
        if (!IsServer)
            return;

        if (player.HasFinished.Value)
            return;

        player.HasFinished.Value = true;

        // Dar puntos según el orden de llegada
        int position = players.Count(p => p.HasFinished.Value);

        switch (position)
        {
            case 1: player.Points.Value += 10; break;
            case 2: player.Points.Value += 7; break;
            case 3: player.Points.Value += 5; break;
            default: player.Points.Value += 3; break;
        }

        CheckRoundEnd();
    }

    private void CheckRoundEnd()
    {
        bool allFinished = players.All(p => p.HasFinished.Value);

        if (allFinished)
        {
            EndRound();
        }
    }

    private void EndRound()
    {
        Debug.Log($"Ronda {currentRound} terminada");

        currentRound++;
        Invoke(nameof(StartNextRound), 5f);
    }

    private void StartNextRound()
    {
        foreach (var player in players)
        {
            player.HasFinished.Value = false;

            // Aquí puedes moverlos al spawn de la siguiente ronda
            // player.transform.position = spawnPoint.position;
        }

        UpdateRoundText();

        Debug.Log($"Comienza la ronda {currentRound}");
    }

    private void UpdateRoundText()
    {
        if (roundText != null)
        {
            roundText.text = $"Round {currentRound}";
        }
    }
}