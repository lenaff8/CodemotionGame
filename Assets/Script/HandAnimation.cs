using UnityEngine;
using UnityEngine;

public class HandAnimation : MonoBehaviour
{
    [Header("Tap Animation")]
    [SerializeField] private float tapRotationAngle = 10f;
    [SerializeField] private float tapDuration = 0.15f;
    [SerializeField] private float returnDuration = 0.15f;
    [SerializeField] private float pauseDuration = 0.4f;

    private float phaseTimer = 0f;
    private int phase = 0; // 0-3 = animación, 4 = pausa
    private Quaternion originalRotation;

    private void Start()
    {
        originalRotation = transform.localRotation;
    }

    private void Update()
    {
        phaseTimer += Time.deltaTime;

        switch (phase)
        {
            case 0: // Tap 1: rotar hacia adelante
                UpdateTapForward(tapDuration);
                break;

            case 1: // Retorno 1: rotar hacia atrás
                UpdateTapReturn(returnDuration);
                break;

            case 2: // Tap 2: rotar hacia adelante
                UpdateTapForward(tapDuration);
                break;

            case 3: // Retorno 2: rotar hacia atrás
                UpdateTapReturn(returnDuration);
                break;

            case 4: // Pausa
                if (phaseTimer >= pauseDuration)
                {
                    phaseTimer = 0f;
                    phase = 0;
                }
                else
                {
                    transform.localRotation = originalRotation;
                }
                break;
        }
    }

    private void UpdateTapForward(float duration)
    {
        if (phaseTimer >= duration)
        {
            // Completar esta fase
            phaseTimer = 0f;
            phase++;
        }
        else
        {
            // Animar la rotación hacia adelante
            float progress = phaseTimer / duration;
            float easeProgress = EaseInOutQuad(progress);
            float rotation = Mathf.Lerp(0f, tapRotationAngle, easeProgress);
            transform.localRotation = originalRotation * Quaternion.Euler(rotation, 0f, 0f);
        }
    }

    private void UpdateTapReturn(float duration)
    {
        if (phaseTimer >= duration)
        {
            // Completar esta fase
            phaseTimer = 0f;
            phase++;

            // Asegurar que volvemos a la posición original
            transform.localRotation = originalRotation;

            // Si completamos los 4 fases, pasar a pausa
            if (phase == 4)
            {
                phase = 4;
            }
        }
        else
        {
            // Animar la rotación de regreso
            float progress = phaseTimer / duration;
            float easeProgress = EaseInOutQuad(progress);
            float rotation = Mathf.Lerp(tapRotationAngle, 0f, easeProgress);
            transform.localRotation = originalRotation * Quaternion.Euler(rotation, 0f, 0f);
        }
    }

    private float EaseInOutQuad(float t)
    {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }
}
