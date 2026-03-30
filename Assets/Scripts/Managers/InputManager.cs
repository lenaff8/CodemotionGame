using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction interactAction;
    private InputAction shootAction;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeInput();
    }

    private void InitializeInput()
    {
        playerInput = GetComponent<PlayerInput>();
        
        if (playerInput != null)
        {
            var actionMap = playerInput.actions;
            moveAction = actionMap.FindAction("Move");
            jumpAction = actionMap.FindAction("Jump");
            interactAction = actionMap.FindAction("Interact");
            shootAction = actionMap.FindAction("Shoot");
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// Obtiene el vector de movimiento (WASD o Joystick analógico)
    /// </summary>
    public Vector2 GetMovementInput()
    {
        return moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
    }

    /// <summary>
    /// Comprueba si se presionó el botón de salto
    /// </summary>
    public bool GetJumpInput()
    {
        return jumpAction?.WasPressedThisFrame() ?? false;
    }

    /// <summary>
    /// Comprueba si se mantiene presionado el botón de salto
    /// </summary>
    public bool GetJumpHeld()
    {
        return jumpAction?.IsPressed() ?? false;
    }

    /// <summary>
    /// Comprueba si se soltó el botón de salto
    /// </summary>
    public bool GetJumpReleased()
    {
        return jumpAction?.WasReleasedThisFrame() ?? false;
    }

    /// <summary>
    /// Comprueba si se presionó el botón de interacción
    /// </summary>
    public bool GetInteractInput()
    {
        return interactAction?.WasPressedThisFrame() ?? false;
    }

    /// <summary>
    /// Comprueba si se presionó el botón de disparo
    /// </summary>
    public bool GetShootInput()
    {
        return shootAction?.WasPressedThisFrame() ?? false;
    }

    /// <summary>
    /// Comprueba si se mantiene presionado el botón de disparo
    /// </summary>
    public bool GetShootHeld()
    {
        return shootAction?.IsPressed() ?? false;
    }

    /// <summary>
    /// Habilita o deshabilita el mapa de acciones
    /// </summary>
    public void SetInputEnabled(bool enabled)
    {
        if (playerInput != null)
        {
            if (enabled)
                playerInput.actions.Enable();
            else
                playerInput.actions.Disable();
        }
    }

    /// <summary>
    /// Cambia el esquema de controles (Keyboard, Gamepad, etc.)
    /// </summary>
    public void SetControlScheme(string schemeName)
    {
        if (playerInput != null)
        {
            playerInput.SwitchCurrentControlScheme(schemeName);
        }
    }

    /// <summary>
    /// Obtiene el esquema de controles actual
    /// </summary>
    public string GetCurrentControlScheme()
    {
        return playerInput?.currentControlScheme ?? "Unknown";
    }

    /// <summary>
    /// Registra un callback para el evento de movimiento
    /// </summary>
    public void OnMoveAction(System.Action<InputAction.CallbackContext> callback)
    {
        if (moveAction != null)
            moveAction.performed += callback;
    }

    /// <summary>
    /// Desregistra un callback del evento de movimiento
    /// </summary>
    public void OffMoveAction(System.Action<InputAction.CallbackContext> callback)
    {
        if (moveAction != null)
            moveAction.performed -= callback;
    }
}
