using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour, InputSystem_Actions.IPlayerActions, InputSystem_Actions.IUIActions
{
    public static InputManager Instance;

    private InputSystem_Actions input;

    // OUTPUTS (lo que usarás desde otros scripts)
    public bool ClickPressed { get; private set; }
    public bool ClickReleased { get; private set; }
    public Vector2 PointerPosition { get; private set; }

    private bool wasPointerDown;

    private void Awake()
    {
        // Singleton básico
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        input = new InputSystem_Actions();

        // Nos suscribimos SOLO a lo necesario
        input.Player.SetCallbacks(this);
        input.UI.SetCallbacks(this);
    }

    private void OnEnable()
    {
        input.Enable();
    }

    private void OnDisable()
    {
        input.Disable();
    }

    private void LateUpdate()
    {
        // Reset flags cada frame
        ClickPressed = false;
        ClickReleased = false;
    }

    private void Update()
    {
        bool isPointerDown = false;

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
        {
            var touch = Touchscreen.current.primaryTouch;
            PointerPosition = touch.position.ReadValue();
            isPointerDown = touch.press.isPressed;
        }
        else if (Mouse.current != null)
        {
            PointerPosition = Mouse.current.position.ReadValue();
            isPointerDown = Mouse.current.leftButton.isPressed;
        }
        else if (Pointer.current != null)
        {
            PointerPosition = Pointer.current.position.ReadValue();
            isPointerDown = Pointer.current.press.isPressed;
        }

        if (isPointerDown && !wasPointerDown)
            ClickPressed = true;

        if (!isPointerDown && wasPointerDown)
            ClickReleased = true;

        wasPointerDown = isPointerDown;
    }

    // =========================
    // CLICK (Mouse + Touch)
    // =========================

    public void OnAttack(InputAction.CallbackContext context)
    {
    }

    public void OnClick(InputAction.CallbackContext context)
    {
    }

    // =========================
    // POSICIÓN (Mouse / Touch)
    // =========================

    public void OnPoint(InputAction.CallbackContext context)
    {
    }

    // =========================
    // IGNORAR TODO LO DEMÁS
    // =========================

    public void OnMove(InputAction.CallbackContext context) { }
    public void OnLook(InputAction.CallbackContext context) { }
    public void OnInteract(InputAction.CallbackContext context) { }
    public void OnCrouch(InputAction.CallbackContext context) { }
    public void OnJump(InputAction.CallbackContext context) { }
    public void OnPrevious(InputAction.CallbackContext context) { }
    public void OnNext(InputAction.CallbackContext context) { }
    public void OnSprint(InputAction.CallbackContext context) { }

    public void OnNavigate(InputAction.CallbackContext context) { }
    public void OnSubmit(InputAction.CallbackContext context) { }
    public void OnCancel(InputAction.CallbackContext context) { }
    public void OnRightClick(InputAction.CallbackContext context) { }
    public void OnMiddleClick(InputAction.CallbackContext context) { }
    public void OnScrollWheel(InputAction.CallbackContext context) { }
    public void OnTrackedDevicePosition(InputAction.CallbackContext context) { }
    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context) { }
}