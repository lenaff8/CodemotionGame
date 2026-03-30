using UnityEngine;

/// <summary>
/// Ejemplos avanzados de cómo usar el sistema CircularUI
/// Descomenta los ejemplos que quieras probar
/// </summary>

// EJEMPLO 1: Control básico de una imagen circular
/*
public class BasicCircularUIExample : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;

    private void Update()
    {
        // Cambiar radio con teclas arriba/abajo
        if (Input.GetKeyDown(KeyCode.UpArrow))
            circularImage.SetCircleRadius(200);
        
        if (Input.GetKeyDown(KeyCode.DownArrow))
            circularImage.SetCircleRadius(100);
        
        // Resetear con R
        if (Input.GetKeyDown(KeyCode.R))
            circularImage.ResetPosition();
    }
}
*/

// EJEMPLO 2: Múltiples imágenes orbitando a diferentes radios
/*
public class MultiOrbitalUIExample : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator[] orbitalImages;

    private void Start()
    {
        for (int i = 0; i < orbitalImages.Length; i++)
        {
            // Cada imagen orbita a un radio diferente
            orbitalImages[i].SetCircleRadius(150 + (i * 50));
            orbitalImages[i].SetRotationSensitivity(2f);
        }
    }
}
*/

// EJEMPLO 3: Seguidor de ratón con efecto de trail
/*
public class MouseFollowerWithTrail : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private float minMovementToShowTrail = 10f;

    private MouseInputProvider mouseInput;
    private Vector2 previousMousePosition;

    private void Start()
    {
        mouseInput = MouseInputProvider.Instance;
        previousMousePosition = mouseInput.GetMousePosition();
    }

    private void Update()
    {
        Vector2 currentMousePos = mouseInput.GetMousePosition();
        float distance = Vector2.Distance(previousMousePosition, currentMousePos);

        if (distance > minMovementToShowTrail)
        {
            trail.enabled = true;
        }
        else
        {
            trail.enabled = false;
        }

        previousMousePosition = currentMousePos;
    }
}
*/

// EJEMPLO 4: Menú circular con múltiples opciones
/*
public class CircularMenuExample : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator[] menuItems;
    [SerializeField] private float radiusPerItem = 50f;
    [SerializeField] private int selectedIndex = 0;

    private MouseInputProvider mouseInput;

    private void Start()
    {
        mouseInput = MouseInputProvider.Instance;

        // Distribuir items en círculo
        float anglePerItem = 360f / menuItems.Length;
        for (int i = 0; i < menuItems.Length; i++)
        {
            menuItems[i].SetCircleRadius(150);
            menuItems[i].SetRotationSensitivity(2f);
        }
    }

    private void Update()
    {
        // Seleccionar item basado en ángulo del ratón
        float mouseInput = this.mouseInput.GetMouseHorizontalInput();
        
        if (Mathf.Abs(mouseInput) > 0.1f)
        {
            // Lógica para determinar qué item está más cerca del ángulo actual
            // Y aplicar efecto visual (escala, color, etc.)
        }
    }

    public void SelectMenuItem(int index)
    {
        selectedIndex = index;
        Debug.Log($"Menú item seleccionado: {index}");
    }
}
*/

// EJEMPLO 5: Sistema de configuración en tiempo real
/*
public class RealtimeCircularUIConfig : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    
    [Header("Configuración")]
    [SerializeField] [Range(50, 300)] private float radiusConfig = 150f;
    [SerializeField] [Range(0.5f, 5f)] private float sensitivityConfig = 2f;
    [SerializeField] [Range(0.01f, 0.5f)] private float dampingConfig = 0.1f;

    private float lastRadius;
    private float lastSensitivity;
    private float lastDamping;

    private void Update()
    {
        // Aplicar cambios si se modificaron en el Inspector
        if (radiusConfig != lastRadius)
        {
            circularImage.SetCircleRadius(radiusConfig);
            lastRadius = radiusConfig;
        }

        if (sensitivityConfig != lastSensitivity)
        {
            circularImage.SetRotationSensitivity(sensitivityConfig);
            lastSensitivity = sensitivityConfig;
        }

        // El damping requeriría una propiedad pública en CircularImageAnimator
        if (dampingConfig != lastDamping)
        {
            lastDamping = dampingConfig;
        }
    }
}
*/

// EJEMPLO 6: Animación de entrada/salida de la escena
/*
public class CircularUIEntranceAnimation : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private float animationDuration = 1f;
    [SerializeField] private float startRadius = 0f;
    [SerializeField] private float endRadius = 150f;

    private float elapsedTime = 0f;

    private void Start()
    {
        circularImage.SetCircleRadius(startRadius);
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / animationDuration;

        if (progress < 1f)
        {
            // Animar el radio suavemente
            float currentRadius = Mathf.Lerp(startRadius, endRadius, progress);
            circularImage.SetCircleRadius(currentRadius);
        }
    }

    public void ExitAnimation()
    {
        StartCoroutine(ExitAnimationCoroutine());
    }

    private System.Collections.IEnumerator ExitAnimationCoroutine()
    {
        float currentRadius = endRadius;
        for (float t = 0; t < animationDuration; t += Time.deltaTime)
        {
            float progress = t / animationDuration;
            currentRadius = Mathf.Lerp(endRadius, startRadius, progress);
            circularImage.SetCircleRadius(currentRadius);
            yield return null;
        }
    }
}
*/

// EJEMPLO 7: Detector de velocidad de movimiento
/*
public class VelocityBasedCircularUI : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private float slowMovementRadius = 100f;
    [SerializeField] private float fastMovementRadius = 200f;
    [SerializeField] private float velocityThreshold = 100f;

    private MouseInputProvider mouseInput;
    private Vector2 previousPosition;

    private void Start()
    {
        mouseInput = MouseInputProvider.Instance;
        previousPosition = mouseInput.GetMousePosition();
    }

    private void Update()
    {
        Vector2 currentPosition = mouseInput.GetMousePosition();
        float velocity = Vector2.Distance(previousPosition, currentPosition) / Time.deltaTime;

        // Cambiar radio basado en velocidad del ratón
        float targetRadius = velocity > velocityThreshold ? fastMovementRadius : slowMovementRadius;
        circularImage.SetCircleRadius(Mathf.Lerp(circularImage.GetComponentInChildren<RectTransform>().rect.width, targetRadius, 0.1f));

        previousPosition = currentPosition;
    }
}
*/

// EJEMPLO 8: Selector de color circular
/*
public class CircularColorSelector : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private Image selectedColorDisplay;
    [SerializeField] private float huePerDegree = 1f;

    private RectTransform imageRectTransform;
    private float currentAngle = 0f;

    private void Start()
    {
        imageRectTransform = circularImage.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Calcular ángulo actual basado en posición de la imagen
        Vector2 position = imageRectTransform.anchoredPosition;
        currentAngle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;

        // Convertir ángulo a valor HSV
        float hue = (currentAngle % 360f) / 360f;
        Color selectedColor = Color.HSVToRGB(hue, 1f, 1f);

        if (selectedColorDisplay != null)
            selectedColorDisplay.color = selectedColor;
    }
}
*/

// EJEMPLO 9: Sistema de rebote en los límites
/*
public class CircularUIWithBounce : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private float maxRotationAngle = 90f;
    [SerializeField] private float bounceDamping = 0.8f;

    private float currentRotation = 0f;
    private float rotationVelocity = 0f;

    private void Update()
    {
        // Implementar lógica de rebote basada en límites de rotación
        if (Mathf.Abs(currentRotation) > maxRotationAngle)
        {
            rotationVelocity *= -bounceDamping;
            currentRotation = Mathf.Clamp(currentRotation, -maxRotationAngle, maxRotationAngle);
        }
    }
}
*/

// EJEMPLO 10: Integración con eventos
/*
public class CircularUIEventSystem : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;

    public delegate void OnCircularMovement(Vector2 position, float angle);
    public static event OnCircularMovement OnMove;

    private RectTransform imageRectTransform;
    private Vector2 previousPosition;

    private void Start()
    {
        imageRectTransform = circularImage.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 currentPosition = imageRectTransform.anchoredPosition;

        if (currentPosition != previousPosition)
        {
            float angle = Mathf.Atan2(currentPosition.y, currentPosition.x) * Mathf.Rad2Deg;
            OnMove?.Invoke(currentPosition, angle);
            previousPosition = currentPosition;
        }
    }
}

// Listener de eventos
public class CircularUIEventListener : MonoBehaviour
{
    private void OnEnable()
    {
        CircularUIEventSystem.OnMove += HandleCircularMove;
    }

    private void OnDisable()
    {
        CircularUIEventSystem.OnMove -= HandleCircularMove;
    }

    private void HandleCircularMove(Vector2 position, float angle)
    {
        Debug.Log($"Movimiento circular - Posición: {position}, Ángulo: {angle}");
    }
}
*/

public class CircularUIExamples : MonoBehaviour
{
    // Este es un script de referencia con ejemplos comentados
    // Descomenta cualquier ejemplo para probarlo
}
