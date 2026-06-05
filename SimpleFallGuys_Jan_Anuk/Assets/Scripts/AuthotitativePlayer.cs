using Unity.Netcode;
using UnityEngine;

public class AuthoritativePlayer : NetworkBehaviour
{

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 8f;

    Rigidbody rb;
    bool jumpRequested;
    Vector2 moveInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }


    void Update()
    {
        if (!IsOwner)
            return;

        Vector2 input = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        bool jump = Input.GetKeyDown(KeyCode.Space);

        SendInputServerRpc(input, jump);
    }

    [ServerRpc]
    void SendInputServerRpc(Vector2 input, bool jump)
    {
        moveInput = input;

        if (jump)
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        if (!IsServer)
            return;

        Vector3 move = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        if (jumpRequested)
        {
            jumpRequested = false;

            if (IsGrounded())
            {
                rb.AddForce(
                    Vector3.up * jumpForce,
                    ForceMode.Impulse
                );
            }
        }
    }
    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, 0.1f);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, -Vector3.up * .1f);
    }
}