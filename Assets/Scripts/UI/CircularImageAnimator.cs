using UnityEngine;

public class CircularImageAnimator : MonoBehaviour
{
    [Header("Configuración Circular")]
    [SerializeField] private float circleRadius = 100f;
    [SerializeField] private Vector2 circleCenter = Vector2.zero;
    [SerializeField] private float rotationSensitivity = 1f;
    [SerializeField] private float smoothDamping = 0.1f;

    [Header("Rotación de la Imagen")]
    [SerializeField] private bool rotateImage = true;
    [SerializeField] private float imageRotationSpeed = 2f;

    private RectTransform rectTransform;
    private MouseInputProvider mouseInput;
    private float currentAngle = 0f;
    private float velocityAngle = 0f;
    private Vector2 currentPosition;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        mouseInput = MouseInputProvider.Instance;

        if (mouseInput == null)
        {
            Debug.LogError("MouseInputProvider no encontrado. Asegúrate de que esté en la escena.");
        }

        // Posición inicial
        currentPosition = rectTransform.anchoredPosition;
        currentAngle = Mathf.Atan2(currentPosition.y - circleCenter.y, currentPosition.x - circleCenter.x) * Mathf.Rad2Deg;
    }

    private void Update()
    {
        if (mouseInput == null)
            return;

        HandleCircularMovement();
        UpdateImageRotation();
    }

    private void HandleCircularMovement()
    {
        float mouseHorizontalInput = mouseInput.GetMouseHorizontalInput();

        // Actualizar el ángulo basado en el movimiento del ratón
        float targetAngleVelocity = mouseHorizontalInput * rotationSensitivity * 360f;
        velocityAngle = Mathf.Lerp(velocityAngle, targetAngleVelocity, smoothDamping);

        // Aplicar el movimiento suave al ángulo
        currentAngle += velocityAngle * Time.deltaTime;

        // Calcular la posición circular basada en el ángulo
        float radians = currentAngle * Mathf.Deg2Rad;
        float x = circleCenter.x + Mathf.Cos(radians) * circleRadius;
        float y = circleCenter.y + Mathf.Sin(radians) * circleRadius;

        currentPosition = new Vector2(x, y);
        rectTransform.anchoredPosition = currentPosition;
    }

    private void UpdateImageRotation()
    {
        if (!rotateImage)
            return;

        // Rotar la imagen en la dirección del movimiento
        float rotationZ = -currentAngle * imageRotationSpeed;
        rectTransform.rotation = Quaternion.Euler(0, 0, rotationZ);
    }

    /// <summary>
    /// Establece el radio del círculo
    /// </summary>
    public void SetCircleRadius(float radius)
    {
        circleRadius = radius;
    }

    /// <summary>
    /// Establece el centro del círculo
    /// </summary>
    public void SetCircleCenter(Vector2 center)
    {
        circleCenter = center;
    }

    /// <summary>
    /// Establece la sensibilidad del movimiento
    /// </summary>
    public void SetRotationSensitivity(float sensitivity)
    {
        rotationSensitivity = sensitivity;
    }

    /// <summary>
    /// Habilita o deshabilita la rotación de la imagen
    /// </summary>
    public void SetImageRotation(bool enabled)
    {
        rotateImage = enabled;
    }

    /// <summary>
    /// Reinicia la posición circular
    /// </summary>
    public void ResetPosition()
    {
        currentAngle = 0f;
        velocityAngle = 0f;
        float radians = currentAngle * Mathf.Deg2Rad;
        float x = circleCenter.x + Mathf.Cos(radians) * circleRadius;
        float y = circleCenter.y + Mathf.Sin(radians) * circleRadius;
        rectTransform.anchoredPosition = new Vector2(x, y);
    }
}
