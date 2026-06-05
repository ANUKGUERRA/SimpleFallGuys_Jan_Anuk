using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    public NetworkVariable<FixedString32Bytes> Nickname = new();
    public NetworkVariable<int> Points = new();
}
