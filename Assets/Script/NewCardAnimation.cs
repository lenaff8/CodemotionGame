using UnityEngine;

public class NewCardAnimation : MonoBehaviour
{
    [Header("Rotation (Y axis)")]
    public float rotationDuration = 1f;
    public float rotationAngle = 180f;
    public AnimationCurve rotationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Offset Movement")]
    public float moveDuration = 0.8f;
    public float moveAmountX = 0.5f;
    public float moveAmountY = 0.4f;
    public AnimationCurve moveCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 startLocalPos;

    private float rotT;
    private float moveT;

    private bool playing;
    private DraggableSprite draggableSprite;

    private void Start()
    {
        draggableSprite = GetComponent<DraggableSprite>();
        startLocalPos = transform.localPosition;
    }

    private void Update()
    {
        if (!playing) return;

        // =========================
        // ROTATION (180 → 0)
        // =========================
        rotT += Time.deltaTime / rotationDuration;
        float rotK = rotationCurve.Evaluate(Mathf.Clamp01(rotT));

        float y = Mathf.Lerp(rotationAngle, 0f, rotK);
        transform.localRotation = Quaternion.Euler(0f, y, 0f);

        // =========================
        // OFFSET
        // =========================
        moveT += Time.deltaTime / moveDuration;
        float moveK = moveCurve.Evaluate(Mathf.Clamp01(moveT));

        float offsetX = Mathf.Sin(moveK * Mathf.PI) * -moveAmountX;
        float offsetY = Mathf.Sin(moveK * Mathf.PI) * -moveAmountY;

        transform.localPosition = startLocalPos + new Vector3(offsetX, offsetY, 0f);

        // =========================
        // END
        // =========================
        if (rotT >= 1f && moveT >= 1f)
        {
            playing = false;

            transform.localPosition = startLocalPos;
            transform.localRotation = Quaternion.identity;
            draggableSprite.SetInteractable(true);
        }
    }

    public void Play()
    {
        // 🔥 IMPORTANTE: siempre empezar desde 180
        transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0f);

        rotT = 0f;
        moveT = 0f;
        playing = true;
    }
}
