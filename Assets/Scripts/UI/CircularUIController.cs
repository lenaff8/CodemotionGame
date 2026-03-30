using UnityEngine;
using UnityEngine.UI;

public class CircularUIController : MonoBehaviour
{
    [SerializeField] private CircularImageAnimator circularImage;
    [SerializeField] private Slider radiusSlider;
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider dampingSlider;
    [SerializeField] private Toggle rotateToggle;
    [SerializeField] private Button resetButton;

    [SerializeField] private Text radiusValueText;
    [SerializeField] private Text sensitivityValueText;
    [SerializeField] private Text dampingValueText;

    private void Start()
    {
        if (circularImage == null)
        {
            Debug.LogError("CircularImageAnimator no asignado en el Inspector");
            return;
        }

        // Configurar listeners para los controles
        if (radiusSlider != null)
        {
            radiusSlider.onValueChanged.AddListener(OnRadiusChanged);
            radiusSlider.value = 150f;
        }

        if (sensitivitySlider != null)
        {
            sensitivitySlider.onValueChanged.AddListener(OnSensitivityChanged);
            sensitivitySlider.value = 2f;
        }

        if (dampingSlider != null)
        {
            dampingSlider.onValueChanged.AddListener(OnDampingChanged);
            dampingSlider.value = 0.1f;
        }

        if (rotateToggle != null)
        {
            rotateToggle.onValueChanged.AddListener(OnRotateToggled);
            rotateToggle.isOn = true;
        }

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(OnResetClicked);
        }
    }

    private void OnRadiusChanged(float value)
    {
        circularImage.SetCircleRadius(value);
        if (radiusValueText != null)
            radiusValueText.text = $"Radio: {value:F0}";
    }

    private void OnSensitivityChanged(float value)
    {
        circularImage.SetRotationSensitivity(value);
        if (sensitivityValueText != null)
            sensitivityValueText.text = $"Sensibilidad: {value:F1}";
    }

    private void OnDampingChanged(float value)
    {
        // Este valor se aplicaría directamente en CircularImageAnimator
        // Por ahora solo actualizamos la UI
        if (dampingValueText != null)
            dampingValueText.text = $"Suavizado: {value:F2}";
    }

    private void OnRotateToggled(bool value)
    {
        circularImage.SetImageRotation(value);
    }

    private void OnResetClicked()
    {
        circularImage.ResetPosition();
    }
}
