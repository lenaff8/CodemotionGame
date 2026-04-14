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
public class UserResponseWrapper
{
    public List<UserResponse> entries;
}

[System.Serializable]
public class LeaderboardEntry
{
    public string id;
    public string name;
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

    private const string PREFS_NAME  = "SavedPlayerName";
    private const string PREFS_EMAIL = "SavedPlayerEmail";
    private const string PREFS_ID    = "SavedPlayerId";

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
    [SerializeField] private TextMeshProUGUI playerBestText;
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

        // Restaurar sesión guardada en el navegador (IndexedDB via PlayerPrefs)
        string savedName  = PlayerPrefs.GetString(PREFS_NAME,  "");
        string savedEmail = PlayerPrefs.GetString(PREFS_EMAIL, "");
        string savedId    = PlayerPrefs.GetString(PREFS_ID,    "");

        if (!string.IsNullOrEmpty(savedName) && !string.IsNullOrEmpty(savedEmail))
        {
            PlayerName  = savedName;
            PlayerEmail = savedEmail;
            playerId    = savedId;
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
        feedbackText.text = "Verificando email...";
        StartCoroutine(CheckAndRegisterCoroutine(name, email));
    }

    // Busca el email en el servidor antes de registrar para evitar duplicados
    private IEnumerator CheckAndRegisterCoroutine(string name, string email)
    {
        string searchUrl = $"{USERS_URL}/search?email={UnityWebRequest.EscapeURL(email)}";

        using (UnityWebRequest req = UnityWebRequest.Get(searchUrl))
        {
            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                UserResponse existing = ParseFirstUser(req.downloadHandler.text);
                if (existing != null)
                {
                    Debug.Log($"Email ya existente: {email} — restaurando sesión (id: {existing.id})");
                    feedbackText.text = $"Bienvenido de nuevo, {existing.name}!";
                    yield return new WaitForSeconds(1.2f);
                    RestoreSession(existing.name, email, existing.id, isNew: false);
                    yield break;
                }
            }
            else
            {
                Debug.LogWarning($"No se pudo buscar email, procediendo con registro normal: {req.error}");
            }
        }

        yield return StartCoroutine(RegisterCoroutine(name, email));
    }

    private IEnumerator RegisterCoroutine(string name, string email)
    {
        feedbackText.text = "Registrando...";
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
                Debug.Log($"Registrado: {name} ({email}) — id: {response?.id}");
                RestoreSession(name, email, response?.id, isNew: true);
            }
            else
            {
                Debug.LogError($"Error al registrar: {req.error}\n{req.downloadHandler.text}");
                feedbackText.text = "Error al conectar. Inténtalo de nuevo.";
                registerButton.interactable = true;
            }
        }
    }

    private void RestoreSession(string name, string email, string id, bool isNew)
    {
        PlayerName  = name;
        PlayerEmail = email;
        playerId    = id ?? "";
        PlayerPrefs.SetString(PREFS_NAME,  name);
        PlayerPrefs.SetString(PREFS_EMAIL, email);
        PlayerPrefs.SetString(PREFS_ID,    playerId);
        PlayerPrefs.Save();
        loginPanel.SetActive(false);

        if (isNew && TutorialManager.Instance != null)
            TutorialManager.Instance.TryShowTutorial();
        else
            GameManager.IsPlaying = true;
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
        playerScoreText.text   = $"Tu puntuación: {score} días";
        playerBestText.text    = "";
        leaderboardStatus.text = "Comprobando puntuación...";
        leaderboardLeft.text   = "";
        leaderboardRight.text  = "";

        // Consultar la puntuación actual del jugador en el servidor
        int serverBest = 0;
        if (!string.IsNullOrEmpty(playerId))
        {
            using (UnityWebRequest req = UnityWebRequest.Get($"{USERS_URL}/{playerId}"))
            {
                yield return req.SendWebRequest();

                if (req.result == UnityWebRequest.Result.Success)
                {
                    var user = JsonUtility.FromJson<UserResponse>(req.downloadHandler.text);
                    serverBest = user?.points ?? 0;
                }
                else
                {
                    Debug.LogWarning($"No se pudo obtener puntuación actual: {req.error}");
                }
            }
        }

        if (score > serverBest)
        {
            playerScoreText.text   = $"Tu puntuación: {score} días";
            playerBestText.text    = $"🏆 Nuevo récord personal!";
            leaderboardStatus.text = "Enviando puntuación...";
            yield return StartCoroutine(SubmitScoreCoroutine(score));
            serverBest = score; // actualizar para mostrar el valor correcto
        }
        else
        {
            playerBestText.text    = $"Tu mejor puntuación: {serverBest} días";
            leaderboardStatus.text = "Cargando ranking...";
            Debug.Log($"Score {score} no supera el mejor en servidor ({serverBest}), no se actualiza.");
        }

        yield return StartCoroutine(FetchLeaderboardCoroutine());
    }

    private IEnumerator SubmitScoreCoroutine(int score)
    {
        var body = new UserRequest { name = PlayerName, email = PlayerEmail, points = score };
        string json = JsonUtility.ToJson(body);

        using (UnityWebRequest req = new UnityWebRequest($"{USERS_URL}/{playerId}", "PUT"))
        {
            req.uploadHandler   = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
                Debug.Log($"Nuevo mejor score enviado: {score} — {PlayerName}");
            else
                Debug.LogError($"Error al enviar score: {req.error}\n{req.downloadHandler.text}");
        }
    }

    private IEnumerator FetchLeaderboardCoroutine()
    {
        leaderboardStatus.text = "Cargando ranking...";

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
            sbLeft.AppendLine($"{i + 1}.  {entries[i].points} días");
            sbRight.AppendLine(entries[i].name);
        }
        leaderboardLeft.text  = sbLeft.ToString().TrimEnd();
        leaderboardRight.text = sbRight.ToString().TrimEnd();
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

    private UserResponse ParseFirstUser(string json)
    {
        try
        {
            string wrapped = "{\"entries\":" + json + "}";
            var wrapper = JsonUtility.FromJson<UserResponseWrapper>(wrapped);
            if (wrapper?.entries != null && wrapper.entries.Count > 0)
                return wrapper.entries[0];
        }
        catch { }
        return null;
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

    private bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }
}
