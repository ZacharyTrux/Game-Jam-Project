using UnityEngine;

public class Player : MonoBehaviour
{
    // Singleton Instance, persistent through scenes
    public static Player Instance { get; private set; }

    // Public Fields


    // Private Fields
    private InputSystem_Actions playerInput;

    private void Awake()
    {
        playerInput = new();
        Instance = this;
        DontDestroyOnLoad(this);
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerInput.Enable();
        playerInput.Player.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayer()
    {

    }

    public void ControlEnemies()
    {

    }

    public void EnableInput()
    {
        playerInput.Player.Enable();
    }
    public void DisableInput()
    {
        playerInput.Player.Disable();
    }
}
