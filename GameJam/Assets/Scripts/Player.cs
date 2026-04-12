using System.Net;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour
{
    public static Player Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] public float moveSpeed = 5f;
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

    public float health = 100f;
    public float maxHealth = 100f;
    public float regenRate = 10f;

    public Slider healthBar;

    private SpriteRenderer spriteRenderer;
    private float lastHorizontal = 0f;
    public int damage = 0;

    // Possession hold
    private PossessionBar currentTargetBar = null;

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
        playerInput.Player.Possess.started  += OnPossessStarted;
        playerInput.Player.Possess.canceled += OnPossessCanceled;
        playerInput.Player.Attack.performed += OnPushBack;
        spriteRenderer = GetComponent<SpriteRenderer>();
        healthBar.maxValue = maxHealth;
    }

    void OnDestroy()
    {
        playerInput.Player.Possess.started  -= OnPossessStarted;
        playerInput.Player.Possess.canceled -= OnPossessCanceled;
        playerInput.Player.Attack.performed -= OnPushBack;
    }

    private void Update()
    {
        if(health < maxHealth)
        {
            health += regenRate * Time.deltaTime;
            health = Mathf.Min(health, maxHealth);
        }
        healthBar.value = health;
    }

    void FixedUpdate()
    {
        Vector2 targetInput = playerInput.Player.Move.ReadValue<Vector2>().normalized;
        moveInput = Vector2.SmoothDamp(moveInput, targetInput, ref currentVelocity, smoothTime);
        rb.linearVelocity = moveSpeed * moveInput;
        if (moveInput.x != 0) lastHorizontal = moveInput.x;
        spriteRenderer.flipX = (lastHorizontal > 0);
    }

    private void OnPossessStarted(InputAction.CallbackContext ctx)
    {
        if (MindControl.Instance == null || !MindControl.Instance.CanControl) return;

        Vector2 worldPos = mainCam.ScreenToWorldPoint(
            UnityEngine.InputSystem.Mouse.current.position.ReadValue()
        );
        RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero, 0f, enemyLayer);
        if (hit.collider == null) return;

        if (!hit.collider.TryGetComponent<Enemy>(out var enemy)) return;

        if (enemy.Type == EnemyType.Boss)
        {
            Debug.Log("Cannot control Boss.");
            return;
        }

        if (enemy.Type == EnemyType.Elite && GameManager.Instance.CurrWave < 5)
            return;

        float dist = Vector2.Distance(transform.position, hit.point);
        if (dist > controlRange)
        {
            Debug.Log("Enemy out of range.");
            return;
        }

        PossessionBar bar = hit.collider.GetComponent<PossessionBar>();
        if (bar == null) return;

        currentTargetBar = bar;
        currentTargetBar.SetTargeted(true);
    }

    private void OnPossessCanceled(InputAction.CallbackContext ctx)
    {
        if (currentTargetBar != null)
        {
            currentTargetBar.SetTargeted(false);
            currentTargetBar = null;
        }
    }

    private void OnPushBack(InputAction.CallbackContext ctx)
    {
        GameObject attack = Instantiate(pushBackEffect, transform.position, Quaternion.identity);
        Destroy(attack, 3f);
        SoundManager.Instance?.PlayPushBack();
    }

    public void EnableInput()  => playerInput.Player.Enable();
    public void DisableInput() => playerInput.Player.Disable();

    public void ResetPlayer()
    {
        rb.linearVelocity = Vector2.zero;
        moveInput = Vector2.zero;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Attack"))
        {
            health -= 20f;
            Debug.Log("Player damaged with remaining health: " + health);
            //if (SceneManager.GetActiveScene().name == "EndlessMode")
                //EndlessModeUI.Instance.UpdateHealth();
        }
        if (health <= 0f)
        {
            Destroy(gameObject);
            if (SceneManager.GetActiveScene().name == "EndlessMode")
                EndlessModeUI.Instance.GameOver();
        }
    }

    public void RegenerateHealth(float amount)
    {
        health = Mathf.Min(maxHealth, health + amount);
    }

    public void IncreaseMoveSpeed(float amount){
        moveSpeed += amount;
    }

    public void IncreaseHealth(float bonusHealth, float bonusRegen){
        maxHealth += bonusHealth;
        regenRate += bonusRegen;
        healthBar.maxValue = maxHealth;
    }

    public void ResetGame()
    {
    }
}