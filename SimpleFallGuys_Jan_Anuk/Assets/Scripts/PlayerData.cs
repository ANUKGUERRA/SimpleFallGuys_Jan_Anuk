using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> Nickname = new();
    public NetworkVariable<int> Points = new();
    public NetworkVariable<bool> HasFinished = new(false);
    public Vector3 InitialPosition { get; private set; }

    public override void OnNetworkSpawn()
    {
            InitialPosition = transform.position;
    }
}
