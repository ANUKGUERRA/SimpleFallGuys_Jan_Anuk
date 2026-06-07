using UnityEngine;

public class FinishLine : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!Unity.Netcode.NetworkManager.Singleton.IsServer)
            return;

        if (other.TryGetComponent<PlayerData>(out var player))
        {
            if (player.HasFinished.Value)
                return;

            FindFirstObjectByType<GameManager>()
                .PlayerFinished(player);
        }
    }
}
