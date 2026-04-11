using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float smoothTime = 0.1f;

    [Header("Mind Control")]
    public float controlRange = 8f;
    public LayerMask enemyLayer;

    [Header("Push Back")]
    public GameObject pushBackEffect;

    private InputSystem_Actions playerInput;
    private Rigidbody2D rb;
    private Vector2 moveInput;
    private Vector2 currentVelocity;
    private Camera mainCam;
    private float health = 100f;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(this);

        playerInput = new InputSystem_Actions();
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void Start(){
        playerInput.Enable();
        playerInput.Player.Enable();
        playerInput.Player.Possess.performed += OnTryPossess;
        playerInput.Player.Attack.performed += OnPushBack;
    }

    void OnDestroy(){
        playerInput.Player.Attack.performed -= OnPushBack;
    }

    void Update()
    {
        Vector2 targetInput = playerInput.Player.Move.ReadValue<Vector2>().normalized;
        moveInput = Vector2.SmoothDamp(moveInput, targetInput, ref currentVelocity, smoothTime);
        transform.Translate(moveSpeed * Time.deltaTime * moveInput);
    }


    private void OnTryPossess(InputAction.CallbackContext ctx)
    {
        if (MindControl.Instance == null || !MindControl.Instance.CanControl) return;

        Vector2 worldPos = mainCam.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue()
            );
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, enemyLayer);

        if (hit.collider == null) return;

        Enemy enemy = hit.collider.GetComponent<Enemy>();
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

    private void OnPushBack(InputAction.CallbackContext ctx){
        GameObject attack = Instantiate(pushBackEffect, transform.position, Quaternion.identity);
        Destroy(attack, 3f);
    }

    public void EnableInput()  => playerInput.Player.Enable();
    public void DisableInput() => playerInput.Player.Disable();

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    public void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.CompareTag("Attack")){
            health -= 20f; 
            Debug.Log("Player damaged with remaining health: " + health);
        }
        if(health <= 0f){
            Destroy(gameObject);
        }
    }

    public void RegenerateHealth(float amount)
    {
        // Hook into your health system when ready
    }
}