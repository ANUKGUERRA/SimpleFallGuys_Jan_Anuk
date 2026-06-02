using System;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class AuthotitativePlayer : NetworkBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;

    private NetworkVariable<Vector3> serverPosition = new(readPerm: NetworkVariableReadPermission.Everyone,
        writePerm: NetworkVariableWritePermission.Server);

    private Vector2 input;
    private float inputSendInterval = 1f / 60f;
    private float inputSendTimer;

    private struct Snapshot
    {
        public Vector3 pos;
        public float time;

        public Snapshot(Vector3 p, float t)
        {
            pos = p;
            time = t;
        }
    }

    private Queue<Snapshot> snapshots = new();

    private void Update()
    {
        if (IsOwner)
        {
            inputSendTimer += Time.deltaTime;
            if (inputSendTimer >= inputSendInterval)
            {
                input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
                SendInputServerRpc(input);
                inputSendTimer = 0;
            }
        }

        if (snapshots.Count >= 2)
        {
            Interpolate();
        }
    }

    private void Interpolate()
    {
        Snapshot[] array = snapshots.ToArray();
        Snapshot from = array[0];
        Snapshot to = array[1];

        float duration = to.time - from.time;
        float elapsed = Time.time - from.time;
        float t = Mathf.Clamp01(elapsed / duration);

        transform.position = Vector3.Lerp(from.pos, to.pos, t);
        if (t >= 1f) snapshots.Dequeue();
    }

    [ServerRpc]
    private void SendInputServerRpc(Vector2 input)
    {
        float deltaTime = NetworkManager.Singleton.ServerTime.FixedDeltaTime;
        Vector3 move = new Vector3(input.x, 0f, input.y) * moveSpeed * deltaTime;
        Vector3 targetPos = transform.position + move;

        transform.position = targetPos;
        serverPosition.Value = targetPos;
    }

    public override void OnNetworkSpawn()
    {
        if (IsClient)
        {
            serverPosition.OnValueChanged += (oldVal, newVal) =>
            {
                snapshots.Enqueue(new Snapshot(newVal, Time.time));
                while (snapshots.Count > 5)
                {
                    snapshots.Dequeue();
                }
            };
        }
    }
}
