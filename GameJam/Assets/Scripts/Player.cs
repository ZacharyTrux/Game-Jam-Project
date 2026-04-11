using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Mind Control")]
    public float controlRange = 8f;
    public LayerMask enemyLayer;

    private InputSystem_Actions playerInput;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Camera mainCam;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        playerInput = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Start()
    {
        playerInput.Enable();
        playerInput.Player.Enable();
        playerInput.Player.Attack.performed += OnTryPossess;
    }

    void OnDestroy()
    {
        playerInput.Player.Attack.performed -= OnTryPossess;
    }

    void Update()
    {
        Vector2 raw = playerInput.Player.Move.ReadValue<Vector2>();
        moveInput = raw.normalized;
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveInput * moveSpeed;
    }

    private void OnTryPossess(InputAction.CallbackContext ctx)
    {
        if (MindControl.Instance == null) return;
        if (!MindControl.Instance.CanControl) return;

        Vector2 worldPos = mainCam.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            );
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, enemyLayer);

        if (hit.collider == null) return;

        EnemyScript enemy = hit.collider.GetComponent<EnemyScript>();
        if (enemy == null) return;

        if (enemy.Type == EnemyType.Boss || enemy.Type == EnemyType.Elite)
        {
            Debug.Log("Cannot control Boss or Elite enemies.");
            return;
        }

        float dist = Vector2.Distance(transform.position, hit.point);
        if (dist > controlRange)
        {
            Debug.Log("Enemy out of range.");
            return;
        }

        MindControl.Instance.TryControl(enemy);
    }

    public void EnableInput()  => playerInput.Player.Enable();
    public void DisableInput() => playerInput.Player.Disable();

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    public void RegenerateHealth(float amount)
    {
        // Hook into your health system when ready
    }
}