using Unity.Netcode;
using UnityEngine;

public enum Direction
{
    Left,
    Right,
}
public class MortalBlade : NetworkBehaviour
{
    [SerializeField] Direction startDirection;

    [SerializeField] private float rotationSpeed = 100f;
    [SerializeField] private float maxAngle = 45f;
    private float currentDirection;

    private void Start()
    {
        // Left = negative rotation
        // Right = positive rotation
        currentDirection = startDirection == Direction.Left ? -1f : 1f;
    }

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        transform.Rotate(0f, 0f, rotationSpeed * currentDirection * Time.fixedDeltaTime);

        float zAngle = transform.localEulerAngles.z;

        if (zAngle > 180f)
            zAngle -= 360f;

        if (zAngle >= maxAngle)
        {
            currentDirection = -1f;
        }
        else if (zAngle <= -maxAngle)
        {
            currentDirection = 1f;
        }
    }
}
