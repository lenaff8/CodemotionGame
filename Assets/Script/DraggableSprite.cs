using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class DraggableSprite : MonoBehaviour
{
    [Header("Center")]
    public Transform center;

    [Header("Angular Limits")]
    public float minAngle = -90f;
    public float maxAngle = 90f;

    [Header("Speeds")]
    public float dragSmoothSpeed = 12f;
    public float returnSpeed = 8f;
    public float snapSpeed = 14f;
    public float positionFollowSpeed = 10f;

    [Header("Threshold")]
    public float returnThreshold = 20f;

    [Header("Snap Return")]
    public float snapReturnDelay = 1f;

    [Header("Card Animation Delay")]
    public float cardAnimDelay = 1f;

    public bool rotateObject = true;
    public Transform coverCard;

    private Camera cam;

    [Header("Fill")]
    public SpriteRenderer fillSprite;
    public float fillSpeed = 5f;
    private float fillAmount = 0f;

    [Header("Text Alpha")]
    public TextMeshProUGUI responseLeft;
    public TextMeshProUGUI responseRight;
    public float alphaDuration = 0.3f;
    public AnimationCurve alphaAnimationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    private float textAlpha = 0f;
    private float alphaTimer = 0f;
    private float alphaTarget = 0f;
    private bool isRightPrevious = false;

    private CardScenario cardScenario;
    
    private enum State
    {
        Idle,
        Dragging,
        Returning,
        Snapping,
        SnapWaiting,
        CardWaiting
    }

    private State state = State.Idle;

    private float radius;

    private float baseAngle;     
    private float startAngle;    
    private float currentAngle;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private Vector2 targetWorldPos;

    private float snapTimer;
    private float cardTimer;

    [Header("Game Over Card")]
    [SerializeField] private float gameOverDelay = 3.5f;
    [SerializeField] private SpriteRenderer cardFaceRenderer; // opcional
    [SerializeField] private Sprite[] gameOverSprites;        // opcional, 8 sprites: Energy_min/max, People_min/max, Rep_min/max, Money_min/max

    private NewCardAnimation newCardAnimation;
    private bool interactable = false;
    private bool pendingPlay = false;
    private int gameOverStep = 0; // 0=normal, 1=última carta snap→mostrará game over, 2=carta game over activa
    
    private void Awake()
    {
        cam = Camera.main;
        ScenarioManager.Instance.onScenarioGenerated += StartDelay;
        GameManager.Instance.onGameOverCard += OnGameOverCard;
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameOverCard -= OnGameOverCard;
    }

    public void SetInteractable(bool interactable)
    {
        this.interactable = interactable;
    }

    private void OnGameOverCard(GameManager.StatType stat, bool exceeded)
    {
        gameOverStep = 1;

        string sentence = GetGameOverPhrase(stat, exceeded);
        cardScenario.SetTexts(sentence, "", "");

        if (cardFaceRenderer != null && gameOverSprites != null)
        {
            int index = (int)stat * 2 + (exceeded ? 1 : 0);
            if (index < gameOverSprites.Length && gameOverSprites[index] != null)
                cardFaceRenderer.sprite = gameOverSprites[index];
        }
    }

    private string GetGameOverPhrase(GameManager.StatType stat, bool exceeded)
    {
        switch (stat)
        {
            case GameManager.StatType.Energy:
                return exceeded ? "El equipo está al límite del estrés. Nadie puede más."
                                : "Sin energía, el equipo se desmorona.";
            case GameManager.StatType.People:
                return exceeded ? "Demasiada gente. El caos se apodera de la empresa."
                                : "Tu equipo te ha abandonado. Estás solo.";
            case GameManager.StatType.Reputation:
                return exceeded ? "Demasiada fama. Los medios destrozan la empresa."
                                : "Tu reputación está por los suelos. Nadie confía en ti.";
            case GameManager.StatType.Money:
                return exceeded ? "Demasiado dinero. Los inversores se lo llevaron todo."
                                : "La empresa quiebra. No queda ni un euro.";
            default:
                return "La empresa no pudo seguir adelante.";
        }
    }

    private void StartDelay()
    {
        ScenarioManager.Instance.onScenarioGenerated -= StartDelay;

        newCardAnimation = GetComponent<NewCardAnimation>();
        cardScenario = GetComponent<CardScenario>();

        originalPosition = transform.position;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        radius = Vector2.Distance(transform.position, center.position);

        Vector2 dir = (transform.position - center.position).normalized;
        baseAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

        currentAngle = baseAngle;
        startAngle = baseAngle;

        if (GameManager.IsPlaying)
        {
            cardScenario.UpdateScenarioTexts();
            newCardAnimation.Play();
            coverCard.gameObject.SetActive(false);
        }
        else
        {
            pendingPlay = true;
        }
    }

    private void Update()
    {
        if (!GameManager.IsPlaying)
            return;

        if (pendingPlay)
        {
            pendingPlay = false;
            cardScenario.UpdateScenarioTexts();
            coverCard.gameObject.SetActive(false);
            newCardAnimation.Play();
        }

        var input = InputManager.Instance;

        HandleInput(input);

        if (state == State.Dragging)
            HandleDrag(input);

        if (state == State.Returning)
            HandleReturn();

        if (state == State.Snapping)
            HandleSnap();

        if (state == State.SnapWaiting)
            HandleSnapWaiting();

        if (state == State.CardWaiting)
            HandleCardWaiting();
    }

    private void HandleInput(InputManager input)
    {
        if (!interactable)
            return;
        
        if (input.ClickPressed)
        {
            Vector2 worldPos = ScreenToWorld(input.PointerPosition);
            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null && hit.gameObject == gameObject)
            {
                state = State.Dragging;
                targetWorldPos = transform.position;
                Vector2 dir = (transform.position - center.position).normalized;
                currentAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            }
        }

        if (input.ClickReleased && state == State.Dragging)
        {
            float delta = Mathf.Abs(Mathf.DeltaAngle(baseAngle, currentAngle)); 

            if (delta < returnThreshold)
            {
                state = State.Returning;
                startAngle = baseAngle; 
            }
            else
            {
                //añadido sonido
                SoundManager.instance.PlaySwipe();
                float leftAngle = -25f;
                float rightAngle = 25f;

                float distLeft = Mathf.Abs(Mathf.DeltaAngle(currentAngle, leftAngle));
                float distRight = Mathf.Abs(Mathf.DeltaAngle(currentAngle, rightAngle));

                startAngle = (distLeft < distRight) ? leftAngle : rightAngle;

                state = State.Snapping;
                interactable = false;

                if (gameOverStep != 2)
                    GameManager.Instance.ApplyScenarioEffects(currentAngle > 0);
            }

            OnDragEnd();
        }
    }

    private void HandleDrag(InputManager input)
    {
        Vector2 mouseWorld = ScreenToWorld(input.PointerPosition);

        targetWorldPos = Vector2.Lerp(
            targetWorldPos,
            mouseWorld,
            1f - Mathf.Exp(-positionFollowSpeed * Time.deltaTime)
        );

        Vector2 fromCenter = targetWorldPos - (Vector2)center.position;

        float angle = Mathf.Atan2(fromCenter.x, fromCenter.y) * Mathf.Rad2Deg;

        currentAngle = Mathf.Clamp(angle, minAngle, maxAngle);

        float delta = Mathf.Abs(Mathf.DeltaAngle(baseAngle, currentAngle));
        if (state == State.Dragging && InputManager.Instance.ClickPressed)
        {
            delta = 0f;
        }
        bool passed = delta >= returnThreshold;

        UpdateFill(passed ? 1f : 0f);
        
        // Determinar si está inclinado a la izquierda o derecha
        bool isRight = currentAngle > 0;
        bool isCentered = !passed; // true si está en el centro
        UpdateTextAlpha(passed ? 1f : 0f, isRight, isCentered);

        ApplyTransform(dragSmoothSpeed);
    }

    private void HandleReturn()
    {
        currentAngle = Mathf.LerpAngle(
            currentAngle,
            startAngle,
            1f - Mathf.Exp(-returnSpeed * Time.deltaTime)
        );

        ApplyTransform(returnSpeed);

        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, startAngle)) < 0.1f)
        {
            currentAngle = startAngle;
            state = State.Idle;

            transform.position = originalPosition;
            transform.rotation = originalRotation;

            ResetVisuals();
        }
    }

    private void HandleSnap()
    {
        currentAngle = Mathf.LerpAngle(
            currentAngle,
            startAngle,
            1f - Mathf.Exp(-snapSpeed * Time.deltaTime)
        );

        ApplyTransform(snapSpeed);

        if (Mathf.Abs(Mathf.DeltaAngle(currentAngle, startAngle)) < 0.1f)
        {
            currentAngle = startAngle;
            state = State.SnapWaiting;
            snapTimer = 0f;
        }
    }

    private void HandleSnapWaiting()
    {
        snapTimer += Time.deltaTime;

        if (snapTimer >= snapReturnDelay)
        {
            transform.position = originalPosition;
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);

            Vector2 dir = (transform.position - center.position).normalized;
            currentAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

            ResetVisuals();

            if (gameOverStep == 2)
            {
                // Carta game over deslizada → esperar delay y mostrar highscore
                gameOverStep = 0;
                state = State.Idle;
                GameManager.Instance.StartGameOverTimer(gameOverDelay);
                return;
            }

            state = State.CardWaiting;
            cardTimer = 0f;

            if (gameOverStep == 1)
            {
                // Última carta regular snapeada → mostrar carta game over (textos ya seteados)
                gameOverStep = 2;
            }
            else
            {
                cardScenario.UpdateNextScenarioTexts();
            }
        }
    }

    // =========================
    // WAIT → CARD
    // =========================
    private void HandleCardWaiting()
    {
        cardTimer += Time.deltaTime;

        if (cardTimer >= cardAnimDelay)
        {
            state = State.Idle;
            //sonido
            SoundManager.instance.PlayNewCard();
            newCardAnimation.Play();
        }
    }

    // =========================
    // HELPERS
    // =========================
    private void OnDragEnd()
    {
        UpdateFill(0f);
        float delta = Mathf.Abs(Mathf.DeltaAngle(baseAngle, currentAngle));
        bool isCentered = delta < returnThreshold;
        UpdateTextAlpha(0f, currentAngle > 0, isCentered);
    }

    private void ApplyTransform(float speed)
    {
        float rad = currentAngle * Mathf.Deg2Rad;

        Vector2 dir = new Vector2(Mathf.Sin(rad), Mathf.Cos(rad));

        Vector2 newPos = (Vector2)center.position + dir * radius;
        transform.position = newPos;

        if (rotateObject)
        {
            Vector2 toCenter = ((Vector2)center.position - newPos).normalized;

            Quaternion targetRot =
                Quaternion.FromToRotation(-Vector2.up, toCenter);

            transform.rotation = Quaternion.Lerp(
                transform.rotation,
                targetRot,
                1f - Mathf.Exp(-speed * Time.deltaTime)
            );
        }
    }

    private void UpdateFill(float target)
    {
        fillAmount = Mathf.Lerp(
            fillAmount,
            target,
            1f - Mathf.Exp(-fillSpeed * Time.deltaTime)
        );

        if (fillSprite != null)
            fillSprite.size = new Vector2(fillSprite.size.x, fillAmount);
    }

    private void UpdateTextAlpha(float target, bool isRight, bool isCentered = false)
    {
        // Si cambió de lado, resetear el timer
        if (isRight != isRightPrevious)
        {
            alphaTimer = 0f;
            isRightPrevious = isRight;
        }

        // Si el target cambió, resetear el timer
        if (Mathf.Abs(alphaTarget - target) > 0.01f)
        {
            alphaTarget = target;
            alphaTimer = 0f;
        }

        // Si está en el centro, resetear el timer y forzar alpha a 0
        if (isCentered)
        {
            alphaTimer = 0f;
            textAlpha = 0f;
        }
        else
        {
            // Avanzar el timer
            alphaTimer += Time.deltaTime / alphaDuration;
            alphaTimer = Mathf.Clamp01(alphaTimer);

            // Evaluar la curva de animación
            float curveValue = alphaAnimationCurve.Evaluate(alphaTimer);
            
            // Interpolar entre el valor actual y el target usando la curva
            textAlpha = Mathf.Lerp(textAlpha, target, curveValue);
        }

        if (isRight)
        {
            // Lado derecho: responseRight se actualiza, responseLeft a 0
            if (responseRight != null)
            {
                Color c = responseRight.color;
                c.a = textAlpha;
                responseRight.color = c;
            }
            
            if (responseLeft != null)
            {
                Color c = responseLeft.color;
                c.a = 0f;
                responseLeft.color = c;
            }
        }
        else
        {
            // Lado izquierdo: responseLeft se actualiza, responseRight a 0
            if (responseLeft != null)
            {
                Color c = responseLeft.color;
                c.a = textAlpha;
                responseLeft.color = c;
            }
            
            if (responseRight != null)
            {
                Color c = responseRight.color;
                c.a = 0f;
                responseRight.color = c;
            }
        }
    }

    private void ResetVisuals()
    {
        fillAmount = 0f;
        textAlpha = 0f;

        if (fillSprite != null)
            fillSprite.size = new Vector2(fillSprite.size.x, 0f);

        if (responseLeft != null)
        {
            Color c = responseLeft.color;
            c.a = 0f;
            responseLeft.color = c;
        }

        if (responseRight != null)
        {
            Color c = responseRight.color;
            c.a = 0f;
            responseRight.color = c;
        }
    }

    public void ResetForNewGame()
    {
        state = State.Idle;
        interactable = false;
        fillAmount = 0f;
        snapTimer = 0f;
        cardTimer = 0f;
        pendingPlay = false;
        gameOverStep = 0;

        transform.position = originalPosition;
        transform.rotation = Quaternion.Euler(0f, 180f, 0f);

        if (coverCard != null)
            coverCard.gameObject.SetActive(false);

        ResetVisuals();

        // Re-suscribir para que StartDelay arranque con el primer escenario nuevo
        ScenarioManager.Instance.onScenarioGenerated += StartDelay;
    }

    private Vector2 ScreenToWorld(Vector2 screenPos)
    {
        Vector3 world = cam.ScreenToWorldPoint(screenPos);
        return new Vector2(world.x, world.y);
    }
}