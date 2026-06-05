using System.Linq;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerData[] players;
    [SerializeField] private TMP_Text[] leaderboardTexts;

    private void Start()
    {
        players = FindObjectsByType<PlayerData>(
            FindObjectsSortMode.None);
        foreach (var player in players)
        {
            player.Points.OnValueChanged += (_, __) => MostrarRanking();
        }
        MostrarRanking();
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
}
