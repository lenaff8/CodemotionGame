using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public static class LoginCanvasCreator
{
    [MenuItem("Tools/Crear Canvas de Login + Highscore")]
    public static void CreateLoginCanvas()
    {
        if (GameObject.Find("LoginCanvas") != null)
        {
            Debug.LogWarning("LoginCanvas ya existe en la escena.");
            return;
        }

        // ── EventSystem ──────────────────────────────────────────────────────
        if (Object.FindFirstObjectByType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<StandaloneInputModule>();
        }

        // ── Canvas raíz ──────────────────────────────────────────────────────
        var canvasGO = new GameObject("LoginCanvas");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;

        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080, 1920);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0f;

        canvasGO.AddComponent<GraphicRaycaster>();

        // ════════════════════════════════════════════════════════════════════
        // PANEL LOGIN
        // ════════════════════════════════════════════════════════════════════
        var loginPanel = CreatePanel("LoginPanel", canvasGO.transform, new Color(0f, 0f, 0f, 0.85f));

        var loginCard = CreateCard("Card", loginPanel.transform, new Vector2(700f, 900f));

        CreateTitle(loginCard.transform, "Registro", -60f);

        var nameLabelGO = CreateLabel("LabelNombre", loginCard.transform, "Nombre", -200f);
        var nameInputGO = CreateInputField("NameInput", loginCard.transform, "Tu nombre...", -260f);
        nameInputGO.GetComponent<TMP_InputField>().characterLimit = 7;

        var emailLabelGO = CreateLabel("LabelEmail", loginCard.transform, "Email", -400f);
        var emailInputGO = CreateInputField("EmailInput", loginCard.transform, "tu@email.com", -460f);
        emailInputGO.GetComponent<TMP_InputField>().contentType = TMP_InputField.ContentType.EmailAddress;

        // ── Fila de privacidad: Toggle + label + link ────────────────────────
        var (privacyToggle, privacyLabelTMP, privacyLinkBtn) = CreatePrivacyRow(loginCard.transform, -610f);

        var registerBtn = CreateButton("RegisterButton", loginCard.transform, "Empezar a jugar",
            new Color(0.2f, 0.6f, 1f, 1f), -720f, 100f);

        var feedbackGO = CreateUIObject("FeedbackText", loginCard.transform);
        var feedbackTMP = feedbackGO.AddComponent<TextMeshProUGUI>();
        feedbackTMP.text = "";
        feedbackTMP.fontSize = 32;
        feedbackTMP.alignment = TextAlignmentOptions.Center;
        feedbackTMP.color = new Color(1f, 0.4f, 0.4f, 1f);
        var feedbackRT = feedbackGO.GetComponent<RectTransform>();
        feedbackRT.anchorMin = new Vector2(0f, 0f);
        feedbackRT.anchorMax = new Vector2(1f, 0f);
        feedbackRT.pivot = new Vector2(0.5f, 0f);
        feedbackRT.sizeDelta = new Vector2(0f, 55f);
        feedbackRT.anchoredPosition = new Vector2(0f, 30f);

        // ════════════════════════════════════════════════════════════════════
        // PANEL HIGHSCORE (oculto por defecto)
        // ════════════════════════════════════════════════════════════════════
        var highscorePanel = CreatePanel("HighscorePanel", canvasGO.transform, new Color(0f, 0f, 0f, 0.92f));
        highscorePanel.SetActive(false);

        var hsCard = CreateCard("Card", highscorePanel.transform, new Vector2(700f, 900f));

        CreateTitle(hsCard.transform, "Game Over", -60f);

        // Puntuación del jugador
        var playerScoreGO = CreateUIObject("PlayerScoreText", hsCard.transform);
        var playerScoreTMP = playerScoreGO.AddComponent<TextMeshProUGUI>();
        playerScoreTMP.text = "Tu puntuación: 0 días";
        playerScoreTMP.fontSize = 44;
        playerScoreTMP.fontStyle = FontStyles.Bold;
        playerScoreTMP.alignment = TextAlignmentOptions.Center;
        playerScoreTMP.color = new Color(1f, 0.85f, 0.2f, 1f);
        var psRT = playerScoreGO.GetComponent<RectTransform>();
        psRT.anchorMin = new Vector2(0.5f, 1f);
        psRT.anchorMax = new Vector2(0.5f, 1f);
        psRT.pivot = new Vector2(0.5f, 1f);
        psRT.sizeDelta = new Vector2(620f, 60f);
        psRT.anchoredPosition = new Vector2(0f, -170f);

        // Separador
        var sepGO = CreateUIObject("Separator", hsCard.transform);
        var sepImg = sepGO.AddComponent<Image>();
        sepImg.color = new Color(1f, 1f, 1f, 0.15f);
        var sepRT = sepGO.GetComponent<RectTransform>();
        sepRT.anchorMin = new Vector2(0.5f, 1f);
        sepRT.anchorMax = new Vector2(0.5f, 1f);
        sepRT.pivot = new Vector2(0.5f, 1f);
        sepRT.sizeDelta = new Vector2(580f, 2f);
        sepRT.anchoredPosition = new Vector2(0f, -250f);

        // Título ranking
        var rankTitleGO = CreateUIObject("RankingTitle", hsCard.transform);
        var rankTitleTMP = rankTitleGO.AddComponent<TextMeshProUGUI>();
        rankTitleTMP.text = "TOP RANKING";
        rankTitleTMP.fontSize = 36;
        rankTitleTMP.fontStyle = FontStyles.Bold;
        rankTitleTMP.alignment = TextAlignmentOptions.Center;
        rankTitleTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        var rtRT = rankTitleGO.GetComponent<RectTransform>();
        rtRT.anchorMin = new Vector2(0.5f, 1f);
        rtRT.anchorMax = new Vector2(0.5f, 1f);
        rtRT.pivot = new Vector2(0.5f, 1f);
        rtRT.sizeDelta = new Vector2(620f, 50f);
        rtRT.anchoredPosition = new Vector2(0f, -275f);

        // Lista leaderboard
        var lbGO = CreateUIObject("LeaderboardText", hsCard.transform);
        var lbTMP = lbGO.AddComponent<TextMeshProUGUI>();
        lbTMP.text = "Cargando...";
        lbTMP.fontSize = 38;
        lbTMP.alignment = TextAlignmentOptions.Center;
        lbTMP.color = Color.white;
        lbTMP.lineSpacing = 10f;
        var lbRT = lbGO.GetComponent<RectTransform>();
        lbRT.anchorMin = new Vector2(0.5f, 1f);
        lbRT.anchorMax = new Vector2(0.5f, 1f);
        lbRT.pivot = new Vector2(0.5f, 1f);
        lbRT.sizeDelta = new Vector2(640f, 350f);
        lbRT.anchoredPosition = new Vector2(0f, -340f);

        // Botón Reintentar
        var retryBtn = CreateButton("RetryButton", hsCard.transform, "Jugar de nuevo",
            new Color(0.2f, 0.75f, 0.4f, 1f), -790f, 100f);

        // ── Conectar LoginManager ────────────────────────────────────────────
        var lmGO = GameObject.Find("LoginManager") ?? new GameObject("LoginManager");
        var lm = lmGO.GetComponent<LoginManager>() ?? lmGO.AddComponent<LoginManager>();

        var so = new SerializedObject(lm);
        so.FindProperty("loginPanel").objectReferenceValue        = loginPanel;
        so.FindProperty("highscorePanel").objectReferenceValue    = highscorePanel;
        so.FindProperty("nameInput").objectReferenceValue         = nameInputGO.GetComponent<TMP_InputField>();
        so.FindProperty("emailInput").objectReferenceValue        = emailInputGO.GetComponent<TMP_InputField>();
        so.FindProperty("privacyToggle").objectReferenceValue     = privacyToggle;
        so.FindProperty("privacyLabel").objectReferenceValue      = privacyLabelTMP;
        so.FindProperty("registerButton").objectReferenceValue    = registerBtn;
        so.FindProperty("feedbackText").objectReferenceValue      = feedbackTMP;
        so.FindProperty("playerScoreText").objectReferenceValue   = playerScoreTMP;
        so.FindProperty("leaderboardText").objectReferenceValue   = lbTMP;
        so.FindProperty("retryButton").objectReferenceValue       = retryBtn;
        so.ApplyModifiedProperties();

        // Conectar botones y toggles
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            registerBtn.onClick, lm.OnRegisterPressed);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            retryBtn.onClick, lm.OnRetryPressed);
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            privacyToggle.onValueChanged, lm.OnPrivacyToggleChanged, false);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            privacyLinkBtn.onClick, lm.OpenPrivacyLink);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = canvasGO;
        Debug.Log("✓ LoginCanvas con panel de Login y Highscore creado. Guarda con Ctrl+S.");
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static GameObject CreateUIObject(string name, Transform parent)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    private static void StretchFull(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    private static GameObject CreatePanel(string name, Transform parent, Color color)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = color;
        StretchFull(go.GetComponent<RectTransform>());
        return go;
    }

    private static GameObject CreateCard(string name, Transform parent, Vector2 size)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = new Color(0.12f, 0.12f, 0.15f, 1f);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.anchoredPosition = Vector2.zero;
        return go;
    }

    private static void CreateTitle(Transform parent, string text, float yPos)
    {
        var go = CreateUIObject("Title", parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 72;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 1f);
        rt.anchorMax = new Vector2(1f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.anchoredPosition = new Vector2(0f, yPos);
        rt.sizeDelta = new Vector2(0f, 90f);
    }

    private static GameObject CreateLabel(string name, Transform parent, string text, float yPos)
    {
        var go = CreateUIObject(name, parent);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 36;
        tmp.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        tmp.alignment = TextAlignmentOptions.Left;
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(580f, 50f);
        rt.anchoredPosition = new Vector2(0f, yPos);
        return go;
    }

    private static GameObject CreateInputField(string name, Transform parent, string placeholder, float yPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(580f, 90f);
        rt.anchoredPosition = new Vector2(0f, yPos);

        var bg = go.AddComponent<Image>();
        bg.color = new Color(0.2f, 0.2f, 0.25f, 1f);

        var textAreaGO = new GameObject("Text Area");
        textAreaGO.transform.SetParent(go.transform, false);
        var taRT = textAreaGO.AddComponent<RectTransform>();
        taRT.anchorMin = Vector2.zero;
        taRT.anchorMax = Vector2.one;
        taRT.offsetMin = new Vector2(16, 8);
        taRT.offsetMax = new Vector2(-16, -8);
        textAreaGO.AddComponent<RectMask2D>();

        var phGO = new GameObject("Placeholder");
        phGO.transform.SetParent(textAreaGO.transform, false);
        StretchFull(phGO.AddComponent<RectTransform>());
        var phTMP = phGO.AddComponent<TextMeshProUGUI>();
        phTMP.text = placeholder;
        phTMP.fontSize = 40;
        phTMP.color = new Color(0.5f, 0.5f, 0.5f, 1f);
        phTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(textAreaGO.transform, false);
        StretchFull(txtGO.AddComponent<RectTransform>());
        var inputTMP = txtGO.AddComponent<TextMeshProUGUI>();
        inputTMP.fontSize = 40;
        inputTMP.color = Color.white;
        inputTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var field = go.AddComponent<TMP_InputField>();
        field.textViewport = taRT;
        field.textComponent = inputTMP;
        field.placeholder = phTMP;
        field.fontAsset = inputTMP.font;

        return go;
    }

    private static (Toggle toggle, TextMeshProUGUI label, Button linkButton) CreatePrivacyRow(Transform parent, float yPos)
    {
        // Contenedor de la fila
        var rowGO = CreateUIObject("PrivacyRow", parent);
        var rowRT = rowGO.GetComponent<RectTransform>();
        rowRT.anchorMin = new Vector2(0.5f, 1f);
        rowRT.anchorMax = new Vector2(0.5f, 1f);
        rowRT.pivot = new Vector2(0.5f, 1f);
        rowRT.sizeDelta = new Vector2(580f, 90f);
        rowRT.anchoredPosition = new Vector2(0f, yPos);

        // ── Toggle ───────────────────────────────────────────────────────────
        var toggleGO = new GameObject("PrivacyToggle");
        toggleGO.transform.SetParent(rowGO.transform, false);
        var toggleRT = toggleGO.AddComponent<RectTransform>();
        toggleRT.anchorMin = new Vector2(0f, 0.5f);
        toggleRT.anchorMax = new Vector2(0f, 0.5f);
        toggleRT.pivot = new Vector2(0f, 0.5f);
        toggleRT.sizeDelta = new Vector2(55f, 55f);
        toggleRT.anchoredPosition = new Vector2(0f, 0f);

        // Fondo del checkbox
        var bgGO = new GameObject("Background");
        bgGO.transform.SetParent(toggleGO.transform, false);
        var bgRT = bgGO.AddComponent<RectTransform>();
        StretchFull(bgRT);
        var bgImg = bgGO.AddComponent<Image>();
        bgImg.color = new Color(0.25f, 0.25f, 0.3f, 1f);

        // Checkmark
        var checkGO = new GameObject("Checkmark");
        checkGO.transform.SetParent(bgGO.transform, false);
        var checkRT = checkGO.AddComponent<RectTransform>();
        checkRT.anchorMin = new Vector2(0.15f, 0.15f);
        checkRT.anchorMax = new Vector2(0.85f, 0.85f);
        checkRT.offsetMin = Vector2.zero;
        checkRT.offsetMax = Vector2.zero;
        var checkImg = checkGO.AddComponent<Image>();
        checkImg.color = new Color(0.2f, 0.7f, 0.4f, 1f);

        var toggle = toggleGO.AddComponent<Toggle>();
        toggle.targetGraphic = bgImg;
        toggle.graphic = checkImg;
        toggle.isOn = false;

        // ── Texto "He leído y acepto la" ─────────────────────────────────────
        var labelGO = CreateUIObject("Label", rowGO.transform);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = "He leído y acepto la";
        labelTMP.fontSize = 33;
        labelTMP.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0f);
        labelRT.anchorMax = new Vector2(0f, 1f);
        labelRT.pivot = new Vector2(0f, 0.5f);
        labelRT.sizeDelta = new Vector2(350f, 0f);
        labelRT.anchoredPosition = new Vector2(70f, 0f);

        // ── Botón-link "política de privacidad" ──────────────────────────────
        var linkGO = CreateUIObject("PrivacyLink", rowGO.transform);
        var linkImg = linkGO.AddComponent<Image>();
        linkImg.color = Color.clear;
        var linkBtn = linkGO.AddComponent<Button>();
        var linkColors = ColorBlock.defaultColorBlock;
        linkColors.normalColor = Color.white;
        linkColors.highlightedColor = new Color(0.6f, 0.85f, 1f);
        linkColors.pressedColor = new Color(0.4f, 0.6f, 0.9f);
        linkBtn.colors = linkColors;

        var linkRT = linkGO.GetComponent<RectTransform>();
        linkRT.anchorMin = new Vector2(0f, 0f);
        linkRT.anchorMax = new Vector2(0f, 1f);
        linkRT.pivot = new Vector2(0f, 0.5f);
        linkRT.sizeDelta = new Vector2(270f, 0f);
        linkRT.anchoredPosition = new Vector2(425f, 0f);

        var linkTxtGO = CreateUIObject("Text", linkGO.transform);
        var linkTMP = linkTxtGO.AddComponent<TextMeshProUGUI>();
        linkTMP.text = "<u>política de privacidad</u>";
        linkTMP.fontSize = 33;
        linkTMP.color = new Color(0.4f, 0.75f, 1f, 1f);
        linkTMP.alignment = TextAlignmentOptions.MidlineLeft;
        StretchFull(linkTxtGO.GetComponent<RectTransform>());

        return (toggle, labelTMP, linkBtn);
    }

    private static Button CreateButton(string name, Transform parent, string label,
        Color color, float yPos, float height)
    {
        var go = CreateUIObject(name, parent);
        var img = go.AddComponent<Image>();
        img.color = color;

        var btn = go.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = Color.Lerp(color, Color.white, 0.2f);
        colors.pressedColor = Color.Lerp(color, Color.black, 0.2f);
        btn.colors = colors;

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 1f);
        rt.anchorMax = new Vector2(0.5f, 1f);
        rt.pivot = new Vector2(0.5f, 1f);
        rt.sizeDelta = new Vector2(580f, height);
        rt.anchoredPosition = new Vector2(0f, yPos);

        var txtGO = CreateUIObject("Text", go.transform);
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = label;
        tmp.fontSize = 48;
        tmp.fontStyle = FontStyles.Bold;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        StretchFull(txtGO.GetComponent<RectTransform>());

        return btn;
    }
}
