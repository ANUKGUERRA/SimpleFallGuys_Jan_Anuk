using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AuthoritativePlayer : NetworkBehaviour
{
    [SerializeField] Bounds bounds;
    private Camera m_camera;

    [SerializeField] float distance = 5f;
    [SerializeField] float height = 2f;
    [SerializeField] int points = 0;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float jumpForce = 8f;
    [SerializeField] float rotationSpeed = 180f;

    [SerializeField] GameObject shape;

    Rigidbody rb;
    Vector2 moveInput;
    float yawInput;
    float pitch;

    bool jumpRequested;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (!IsOwner)
            return;

        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        if (!IsOwner)
            return;

        Vector2 move = new Vector2(
            Input.GetAxisRaw("Horizontal"),
            Input.GetAxisRaw("Vertical")
        );

        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        bool jump = Input.GetKeyDown(KeyCode.Space);

        SendInputServerRpc(move, mouseX, jump);

        pitch -= mouseY * 2f;
        pitch = Mathf.Clamp(pitch, -80f, 80f);
    }

    [ServerRpc]
    void SendInputServerRpc(Vector2 move, float mouseX, bool jump)
    {
        moveInput = move;
        yawInput = mouseX;

        if (jump)
            jumpRequested = true;
    }

    void FixedUpdate()
    {
        if (!IsServer) return;

        float yaw = yawInput * rotationSpeed * Time.fixedDeltaTime;
        rb.MoveRotation(rb.rotation * Quaternion.Euler(0, yaw, 0));

        Vector3 move = (transform.forward * moveInput.y + transform.right * moveInput.x) * moveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + move);

        if (jumpRequested)
        {
            jumpRequested = false;

            if (IsGrounded())
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private void LateUpdate()
    {
        if (!IsOwner || m_camera == null)
            return;

        UpdateCameraPosition();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;
        if (other.gameObject.tag == "deathZone")
        {
            transform.position = GetRandomFreePosition();
        }

        if (other.gameObject.tag == "powerUp")
        {
            moveSpeed += 2.0f;
            Destroy(other.gameObject);
        }
        else if (other.gameObject.CompareTag("finishLine"))
        {
            moveSpeed = 0;
            jumpForce = 0;
            GetComponent<PlayerData>().HasFinished.Value = true;

            FindFirstObjectByType<GameManager>().CheckRoundEnd();
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsServer)
        {
            transform.position = GetRandomFreePosition();
            shape.SetActive(true);
        }


        if (!IsOwner)
            return;
        m_camera = Camera.main;
        NetworkManager.SceneManager.OnLoadEventCompleted += OnSceneLoaded;
    }

    private void OnSceneLoaded(
        string sceneName,
        LoadSceneMode loadSceneMode,
        System.Collections.Generic.List<ulong> clientsCompleted,
        System.Collections.Generic.List<ulong> clientsTimedOut)
    {
        m_camera = Camera.main;
        NetworkManager.SceneManager.OnLoadEventCompleted -= OnSceneLoaded;
    }

    private void UpdateCameraPosition()
    {
        Quaternion camRotation = transform.rotation * Quaternion.Euler(pitch, 0, 0);

        Vector3 targetPos = transform.position
            + Vector3.up * height
            + camRotation * new Vector3(0, 0, -distance);

        m_camera.transform.position = targetPos;
        m_camera.transform.rotation = camRotation;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -transform.up, 0.35f);
    }
    Vector3 GetRandomFreePosition()
    {
        const int maxAttempts = 50;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector3 position = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            bool occupied = Physics.CheckSphere(
                position,
                2
            );

            Debug.Log("Collision with: " + occupied);
            if (!occupied)
                return position;
        }

        Debug.LogWarning("Could not find a free position.");
        return Vector3.zero;
    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(transform.position, -transform.up, Color.red);
        Debug.DrawRay(transform.position, Vector3.down, Color.blue);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(bounds.center, bounds.size);
    }
}