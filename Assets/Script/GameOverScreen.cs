using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private RectTransform artwork;
    [SerializeField] private float fadeDuration = 0.4f;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool playing;
    private float timer;
    private bool subscribed;
    private bool restarting;

    private void Awake()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        if (artwork != null)
        {
            artwork.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
    }

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
        {
            yield return null;
        }

        GameManager.Instance.onGameOver += Show;
        subscribed = true;
    }

    private void OnDestroy()
    {
        if (subscribed && GameManager.Instance != null)
        {
            GameManager.Instance.onGameOver -= Show;
        }
    }

    private void Update()
    {
        if (!playing)
            return;

        timer += Time.unscaledDeltaTime;

        float fadeT = Mathf.Clamp01(timer / fadeDuration);
        float scaleT = Mathf.Clamp01(timer / scaleDuration);

        canvasGroup.alpha = curve.Evaluate(fadeT);

        if (artwork != null)
        {
            float scale = Mathf.LerpUnclamped(0.8f, 1f, curve.Evaluate(scaleT));
            artwork.localScale = new Vector3(scale, scale, 1f);
        }

        if (fadeT >= 1f && scaleT >= 1f)
        {
            playing = false;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
        }
    }

    private void Show()
    {
        playing = true;
        timer = 0f;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = true;

        if (artwork != null)
        {
            artwork.localScale = new Vector3(0.8f, 0.8f, 1f);
        }
    }

    public void RetryGame()
    {
        if (restarting)
            return;

        StartCoroutine(RetryGameCoroutine());
    }

    private IEnumerator RetryGameCoroutine()
    {
        restarting = true;
        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            Destroy(GameManager.Instance.gameObject);
        }

        if (AIScenarioGenerator.Instance != null)
        {
            Destroy(AIScenarioGenerator.Instance.gameObject);
        }

        yield return null;

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}