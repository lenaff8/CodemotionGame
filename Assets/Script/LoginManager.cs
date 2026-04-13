using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine.Networking;

[System.Serializable]
public class UserRequest
{
    public string name;
    public string email;
    public int points;
}

[System.Serializable]
public class UserResponse
{
    public string id;
    public string name;
    public string email;
    public int points;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string name;
    public string email;
    public int points;
}

[System.Serializable]
public class LeaderboardWrapper
{
    public List<LeaderboardEntry> entries;
}

public class LoginManager : MonoBehaviour
{
    public static LoginManager Instance;

    private const string USERS_URL       = "https://softwareengineering-gzbrg3f6evdpb5ff.canadacentral-01.azurewebsites.net/api/users";
    private const string LEADERBOARD_URL = "https://softwareengineering-gzbrg3f6evdpb5ff.canadacentral-01.azurewebsites.net/api/leaderboard";

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
    [SerializeField] private TextMeshProUGUI leaderboardStatus;
    [SerializeField] private TextMeshProUGUI leaderboardLeft;
    [SerializeField] private TextMeshProUGUI leaderboardRight;
    [SerializeField] private Button retryButton;

    public static string PlayerName  { get; private set; }
    public static string PlayerEmail { get; private set; }
    private static string playerId;

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
        string name  = nameInput.text.Trim();
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
        var body = new UserRequest { name = name, email = email, points = 0 };
        string json = JsonUtility.ToJson(body);

        using (UnityWebRequest req = new UnityWebRequest(USERS_URL, "POST"))
        {
            req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                var response = JsonUtility.FromJson<UserResponse>(req.downloadHandler.text);
                PlayerName  = name;
                PlayerEmail = email;
                playerId    = response?.id;
                Debug.Log($"Registrado: {name} ({email}) — id: {playerId}");
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

        var cardScenario = Object.FindFirstObjectByType<CardScenario>();
        cardScenario?.ClearTexts();

        StartCoroutine(SubmitAndFetch(finalScore));
    }

    private IEnumerator SubmitAndFetch(int score)
    {
        playerScoreText.text = $"Tu puntuación: {score} días";
        leaderboardStatus.text = "Enviando puntuación...";
        leaderboardLeft.text  = "";
        leaderboardRight.text = "";

        yield return StartCoroutine(SubmitScoreCoroutine(score));

        leaderboardStatus.text = "Cargando ranking...";
        yield return StartCoroutine(FetchLeaderboardCoroutine());
    }

    private IEnumerator SubmitScoreCoroutine(int score)
    {
        if (string.IsNullOrEmpty(playerId))
        {
            Debug.LogWarning("Sin ID de usuario, no se puede enviar score.");
            yield break;
        }

        var body = new UserRequest { name = PlayerName, email = PlayerEmail, points = score };
        string json = JsonUtility.ToJson(body);
        string url  = $"{USERS_URL}/{playerId}";

        using (UnityWebRequest req = new UnityWebRequest(url, "PUT"))
        {
            req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log($"Score enviado: {score} — {PlayerName}");
            else
                Debug.LogError($"Error al enviar score: {req.error}\n{req.downloadHandler.text}");
        }
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        using (UnityWebRequest req = UnityWebRequest.Get(LEADERBOARD_URL))
        {
            yield return req.SendWebRequest();

            if (req.result != UnityWebRequest.Result.Success)
            {
                leaderboardStatus.text = "No se pudo cargar el ranking.";
                yield break;
            }

            List<LeaderboardEntry> entries = ParseLeaderboard(req.downloadHandler.text);

            if (entries == null || entries.Count == 0)
            {
                leaderboardStatus.text = "Sin datos en el ranking.";
                yield break;
            }

            leaderboardStatus.text = "";
            RenderLeaderboard(entries);
        }
    }

    private void RenderLeaderboard(List<LeaderboardEntry> entries)
    {
        entries.Sort((a, b) => b.points.CompareTo(a.points));
        var sbLeft  = new System.Text.StringBuilder();
        var sbRight = new System.Text.StringBuilder();
        int top = Mathf.Min(10, entries.Count);
        for (int i = 0; i < top; i++)
        {
            string medal = i == 0 ? "🥇" : i == 1 ? "🥈" : i == 2 ? "🥉" : $"{i + 1}.";
            sbLeft.AppendLine($"{medal}  {entries[i].points} días");
            sbRight.AppendLine(entries[i].name);
        }
        leaderboardLeft.text  = sbLeft.ToString().TrimEnd();
        leaderboardRight.text = sbRight.ToString().TrimEnd();
    }

    private List<LeaderboardEntry> ParseLeaderboard(string json)
    {
        try
        {
            string wrapped = "{\"entries\":" + json + "}";
            var wrapper = JsonUtility.FromJson<LeaderboardWrapper>(wrapped);
            if (wrapper?.entries != null && wrapper.entries.Count > 0)
                return wrapper.entries;
        }
        catch { }

        return null;
    }

    // ── Retry ─────────────────────────────────────────────────────────────────

    public void OnRetryPressed()
    {
        highscorePanel.SetActive(false);

        var gameOverScreen = Object.FindFirstObjectByType<GameOverScreen>();
        gameOverScreen?.Hide();

        GameManager.Instance.ResetGame();
        ScenarioManager.Instance.ResetGame();
    }

    public void OnPrivacyToggleChanged(bool isOn)
    {
        if (isOn)
            privacyLabel.color = new Color(0.8f, 0.8f, 0.8f, 1f);
    }

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
