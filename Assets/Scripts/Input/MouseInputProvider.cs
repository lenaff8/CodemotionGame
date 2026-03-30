using UnityEngine;
using UnityEngine.InputSystem;

public class MouseInputProvider : MonoBehaviour
{
    public static MouseInputProvider Instance { get; private set; }

    private Mouse mouse;
    private Vector2 currentMousePosition;
    private Vector2 previousMousePosition;
    private Vector2 mouseDelta;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        mouse = Mouse.current;
    }

    private void Update()
    {
        if (mouse == null)
            return;

        previousMousePosition = currentMousePosition;
        currentMousePosition = mouse.position.ReadValue();
        mouseDelta = currentMousePosition - previousMousePosition;
    }

    /// <summary>
    /// Obtiene la posición actual del ratón en pantalla
    /// </summary>
    public Vector2 GetMousePosition()
    {
        return currentMousePosition;
    }

    /// <summary>
    /// Obtiene el delta de movimiento del ratón en este frame
    /// </summary>
    public Vector2 GetMouseDelta()
    {
        return mouseDelta;
    }

    /// <summary>
    /// Obtiene el desplazamiento horizontal del ratón (-1 a 1)
    /// </summary>
    public float GetMouseHorizontalInput()
    {
        float deltaX = mouseDelta.x;
        return Mathf.Clamp(deltaX * 0.01f, -1f, 1f);
    }

    /// <summary>
    /// Comprueba si el ratón se está moviendo
    /// </summary>
    public bool IsMouseMoving()
    {
        return mouseDelta.magnitude > 0.1f;
    }
}
