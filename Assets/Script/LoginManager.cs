using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

[System.Serializable]
public class LeaderboardEntry
{
    public string playerName;
    public string email;
    public int score;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public List<LeaderboardEntry> entries;
}

[System.Serializable]
public class LeaderboardResponse
{
    public bool success;
    public List<LeaderboardEntry> data;
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    private const string API_URL = "https://softwareengineering-gzbrg3f6evdpb5ff.canadacentral-01.azurewebsites.net/api/leaderboard";

    [Header("Panels")]
    [SerializeField] private GameObject loginPanel;
    [SerializeField] private GameObject highscorePanel;

    [Header("Login - Campos")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private UnityEngine.UI.Toggle privacyToggle;
    [SerializeField] private TextMeshProUGUI privacyLabel;
    [SerializeField] private Button registerButton;
    [SerializeField] private TextMeshProUGUI feedbackText;

    [Header("Highscore")]
    [SerializeField] private TextMeshProUGUI playerScoreText;
    [SerializeField] private TextMeshProUGUI leaderboardText;
    [SerializeField] private Button retryButton;

    public static string PlayerName { get; private set; }
    public static string PlayerEmail { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    private IEnumerator Start()
    {
        while (GameManager.Instance == null)
            yield return null;

        GameManager.Instance.onGameOver += OnGameOver;

        // Retry: ya registrado, saltar login
        if (!string.IsNullOrEmpty(PlayerName))
        {
            loginPanel.SetActive(false);
            highscorePanel.SetActive(false);
            GameManager.IsPlaying = true;
            yield break;
        }

        loginPanel.SetActive(true);
        highscorePanel.SetActive(false);
        feedbackText.text = "";
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.onGameOver -= OnGameOver;
    }

    // ── Login ─────────────────────────────────────────────────────────────────

    public void OnRegisterPressed()
    {
        string name = nameInput.text.Trim();
        string email = emailInput.text.Trim();

        if (!privacyToggle.isOn)
        {
            feedbackText.text = "Debes aceptar la política de privacidad.";
            privacyLabel.color = new Color(1f, 0.3f, 0.3f, 1f);
            return;
        }

        if (string.IsNullOrEmpty(name))
        {
            feedbackText.text = "Introduce tu nombre.";
            return;
        }

        if (!IsValidEmail(email))
        {
            feedbackText.text = "Email no válido.";
            return;
        }

        registerButton.interactable = false;
        feedbackText.text = "Registrando...";
        StartCoroutine(RegisterCoroutine(name, email));
    }

    private IEnumerator RegisterCoroutine(string name, string email)
    {
        var entry = new LeaderboardEntry { playerName = name, email = email, score = 0 };
        string json = JsonUtility.ToJson(entry);

        using (UnityWebRequest req = new UnityWebRequest(API_URL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                PlayerName = name;
                PlayerEmail = email;
                Debug.Log($"Registrado: {name} ({email})");
                loginPanel.SetActive(false);
                GameManager.IsPlaying = true;
            }
            else
            {
                Debug.LogError($"Error al registrar: {req.error}\n{req.downloadHandler.text}");
                feedbackText.text = "Error al conectar. Inténtalo de nuevo.";
                registerButton.interactable = true;
            }
        }
    }

    // ── Game Over → Highscore ─────────────────────────────────────────────────

    private void OnGameOver()
    {
        int finalScore = GameManager.Instance != null ? GameManager.Instance.Rounds : 0;
        highscorePanel.SetActive(true);
        loginPanel.SetActive(false);
        StartCoroutine(SubmitAndFetch(finalScore));
    }

    private IEnumerator SubmitAndFetch(int score)
    {
        playerScoreText.text = $"Tu puntuación: {score} días";
        leaderboardText.text = "Enviando puntuación...";

        // 1. Enviar score
        yield return StartCoroutine(SubmitScoreCoroutine(score));

        // 2. Mostrar leaderboard
        leaderboardText.text = "Cargando ranking...";
        yield return StartCoroutine(FetchLeaderboardCoroutine());
    }

    private IEnumerator SubmitScoreCoroutine(int score)
    {
        var entry = new LeaderboardEntry { playerName = PlayerName, email = PlayerEmail, score = score };
        string json = JsonUtility.ToJson(entry);

        using (UnityWebRequest req = new UnityWebRequest(API_URL, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log($"Score enviado: {score} — {PlayerName}");
            else
                Debug.LogError($"Error al enviar score: {req.error}");
        }
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(API_URL))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                leaderboardText.text = "No se pudo cargar el ranking.";
                yield break;
            }

            List<LeaderboardEntry> entries = ParseLeaderboard(req.downloadHandler.text);

            if (entries == null || entries.Count == 0)
            {
                leaderboardText.text = "Sin datos en el ranking.";
                yield break;
            }

            // Ordenar por score descendente y mostrar top 5
            entries.Sort((a, b) => b.score.CompareTo(a.score));
            var sb = new System.Text.StringBuilder();
            int top = Mathf.Min(5, entries.Count);
            for (int i = 0; i < top; i++)
            {
                string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
                sb.AppendLine($"{medal}  {entries[i].playerName}  —  {entries[i].score} días");
            }
            leaderboardText.text = sb.ToString().TrimEnd();
        }
    }

    private List<LeaderboardEntry> ParseLeaderboard(string json)
    {
        try
        {
            // Intentar como array directo
            string wrapped = "{\"entries\":" + json + "}";
            var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(wrapped);
            if (wrapper?.entries != null && wrapper.entries.Count > 0)
                return wrapper.entries;
        }
        catch { }

        try
        {
            // Intentar como { success, data: [...] }
            var response = JsonUtility.FromJson<LeaderboardResponse>(json);
            if (response?.data != null && response.data.Count > 0)
                return response.data;
        }
        catch { }

        return null;
    }

    // ── Retry ─────────────────────────────────────────────────────────────────

    public void OnRetryPressed()
    {
        highscorePanel.SetActive(false);

        // Ocultar GameOverScreen si existe
        var gameOverScreen = Object.FindFirstObjectByType<GameOverScreen>();
        gameOverScreen?.Hide();

        // Resetear juego sin recargar escena
        GameManager.Instance.ResetGame();
        ScenarioManager.Instance.ResetGame();
    }

    // Llamado por el Toggle en el Inspector (OnValueChanged)
    public void OnPrivacyToggleChanged(bool isOn)
    {
        if (isOn)
            privacyLabel.color = new Color(0.8f, 0.8f, 0.8f, 1f);
    }

    // Llamado por el botón-link en el Inspector
    public void OpenPrivacyLink()
    {
        Application.OpenURL("https://www.betterask.erni/es-es/privacy-statement/");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
