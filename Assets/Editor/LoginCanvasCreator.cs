using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public static class LoginCanvasCreator
{
    [MenuItem("Tools/Crear Canvas de Login + Highscore + Tutorial")]
    public static void CreateLoginCanvas()
    {
        // Borrar objetos existentes para recrearlos limpios
        var existingCanvas = GameObject.Find("LoginCanvas");
        if (existingCanvas != null) Undo.DestroyObjectImmediate(existingCanvas);

        var existingLM = GameObject.Find("LoginManager");
        if (existingLM != null) Undo.DestroyObjectImmediate(existingLM);

        var existingTM = GameObject.Find("TutorialManager");
        if (existingTM != null) Undo.DestroyObjectImmediate(existingTM);

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

        var loginCard = CreateCard("Card", loginPanel.transform, new Vector2(700f, 1030f));

        CreateTitle(loginCard.transform, "Registro", -60f);

        var nameLabelGO = CreateLabel("LabelNombre", loginCard.transform, "Nombre", -200f);
        var nameInputGO = CreateInputField("NameInput", loginCard.transform, "Tu nombre...", -260f);
        nameInputGO.GetComponent<TMP_InputField>().characterLimit = 7;

        var emailLabelGO = CreateLabel("LabelEmail", loginCard.transform, "Email", -400f);
        var emailInputGO = CreateInputField("EmailInput", loginCard.transform, "tu@email.com", -460f);

        // ── Fila de privacidad ───────────────────────────────────────────────
        var (privacyToggle, privacyLabelTMP, privacyLinkBtn) = CreateCheckboxRow(
            "PrivacyRow", loginCard.transform,
            "He leído y acepto la", "política de privacidad",
            -610f);

        // ── Fila de bases del juego ──────────────────────────────────────────
        var (gameRulesToggle, gameRulesLabelTMP, gameRulesLinkBtn) = CreateCheckboxRow(
            "GameRulesRow", loginCard.transform,
            "He leído y acepto las", "bases del juego",
            -715f);

        var registerBtn = CreateButton("RegisterButton", loginCard.transform, "Empezar a jugar",
            new Color(0.2f, 0.6f, 1f, 1f), -835f, 100f);

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

        var hsCard = CreateCard("Card", highscorePanel.transform, new Vector2(700f, 1200f));

        CreateTitle(hsCard.transform, "Game Over", -60f);

        // Puntuación de esta partida
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

        // Mejor puntuación personal
        var playerBestGO = CreateUIObject("PlayerBestText", hsCard.transform);
        var playerBestTMP = playerBestGO.AddComponent<TextMeshProUGUI>();
        playerBestTMP.text = "";
        playerBestTMP.fontSize = 36;
        playerBestTMP.alignment = TextAlignmentOptions.Center;
        playerBestTMP.color = new Color(0.4f, 0.9f, 0.6f, 1f);
        var pbRT = playerBestGO.GetComponent<RectTransform>();
        pbRT.anchorMin = new Vector2(0.5f, 1f);
        pbRT.anchorMax = new Vector2(0.5f, 1f);
        pbRT.pivot = new Vector2(0.5f, 1f);
        pbRT.sizeDelta = new Vector2(620f, 50f);
        pbRT.anchoredPosition = new Vector2(0f, -235f);

        // Separador
        var sepGO = CreateUIObject("Separator", hsCard.transform);
        var sepImg = sepGO.AddComponent<Image>();
        sepImg.color = new Color(1f, 1f, 1f, 0.15f);
        var sepRT = sepGO.GetComponent<RectTransform>();
        sepRT.anchorMin = new Vector2(0.5f, 1f);
        sepRT.anchorMax = new Vector2(0.5f, 1f);
        sepRT.pivot = new Vector2(0.5f, 1f);
        sepRT.sizeDelta = new Vector2(580f, 2f);
        sepRT.anchoredPosition = new Vector2(0f, -300f);

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
        rtRT.anchoredPosition = new Vector2(0f, -325f);

        // Texto de estado (Cargando... / errores) — centrado, encima de las columnas
        var lbStatusGO = CreateUIObject("LeaderboardStatus", hsCard.transform);
        var lbStatusTMP = lbStatusGO.AddComponent<TextMeshProUGUI>();
        lbStatusTMP.text = "Cargando...";
        lbStatusTMP.fontSize = 34;
        lbStatusTMP.alignment = TextAlignmentOptions.Center;
        lbStatusTMP.color = new Color(0.7f, 0.7f, 0.7f, 1f);
        var lbStatusRT = lbStatusGO.GetComponent<RectTransform>();
        lbStatusRT.anchorMin = new Vector2(0.5f, 1f);
        lbStatusRT.anchorMax = new Vector2(0.5f, 1f);
        lbStatusRT.pivot = new Vector2(0.5f, 1f);
        lbStatusRT.sizeDelta = new Vector2(600f, 50f);
        lbStatusRT.anchoredPosition = new Vector2(0f, -395f);

        // Columna izquierda: puntuación (con medalla)
        var lbLeftGO = CreateUIObject("LeaderboardLeft", hsCard.transform);
        var lbLeftTMP = lbLeftGO.AddComponent<TextMeshProUGUI>();
        lbLeftTMP.text = "";
        lbLeftTMP.fontSize = 34;
        lbLeftTMP.alignment = TextAlignmentOptions.TopLeft;
        lbLeftTMP.color = Color.white;
        lbLeftTMP.lineSpacing = 12f;
        var lbLeftRT = lbLeftGO.GetComponent<RectTransform>();
        lbLeftRT.anchorMin = new Vector2(0f, 1f);
        lbLeftRT.anchorMax = new Vector2(0f, 1f);
        lbLeftRT.pivot = new Vector2(0f, 1f);
        lbLeftRT.sizeDelta = new Vector2(230f, 470f);
        lbLeftRT.anchoredPosition = new Vector2(40f, -460f);

        // Columna derecha: nombre
        var lbRightGO = CreateUIObject("LeaderboardRight", hsCard.transform);
        var lbRightTMP = lbRightGO.AddComponent<TextMeshProUGUI>();
        lbRightTMP.text = "";
        lbRightTMP.fontSize = 34;
        lbRightTMP.alignment = TextAlignmentOptions.TopLeft;
        lbRightTMP.color = Color.white;
        lbRightTMP.lineSpacing = 12f;
        var lbRightRT = lbRightGO.GetComponent<RectTransform>();
        lbRightRT.anchorMin = new Vector2(0f, 1f);
        lbRightRT.anchorMax = new Vector2(0f, 1f);
        lbRightRT.pivot = new Vector2(0f, 1f);
        lbRightRT.sizeDelta = new Vector2(400f, 470f);
        lbRightRT.anchoredPosition = new Vector2(280f, -460f);

        // Botón Reintentar
        var retryBtn = CreateButton("RetryButton", hsCard.transform, "Jugar de nuevo",
            new Color(0.2f, 0.75f, 0.4f, 1f), -1080f, 100f);

        // ════════════════════════════════════════════════════════════════════
        // PANEL TUTORIAL — Panel 1: instrucciones + leyenda de fichas
        // ════════════════════════════════════════════════════════════════════
        var tutorialPanel = CreatePanel("TutorialPanel", canvasGO.transform, new Color(0f, 0f, 0f, 0.88f));
        tutorialPanel.SetActive(false);

        var tutCard = CreateCard("Card", tutorialPanel.transform, new Vector2(700f, 1100f));

        // Texto de instrucción
        var instrGO = CreateUIObject("InstructionText", tutCard.transform);
        var instrTMP = instrGO.AddComponent<TextMeshProUGUI>();
        instrTMP.text = "Haz swap a la derecha e izquierda, toma una decisión y no pierdas las fichas";
        instrTMP.fontSize = 42;
        instrTMP.alignment = TextAlignmentOptions.Center;
        instrTMP.color = Color.white;
        instrTMP.enableWordWrapping = true;
        var instrRT = instrGO.GetComponent<RectTransform>();
        instrRT.anchorMin = new Vector2(0.5f, 1f);
        instrRT.anchorMax = new Vector2(0.5f, 1f);
        instrRT.pivot = new Vector2(0.5f, 1f);
        instrRT.sizeDelta = new Vector2(600f, 250f);
        instrRT.anchoredPosition = new Vector2(0f, -60f);

        // Separador
        var tutSepGO = CreateUIObject("Separator", tutCard.transform);
        var tutSepImg = tutSepGO.AddComponent<Image>();
        tutSepImg.color = new Color(1f, 1f, 1f, 0.15f);
        var tutSepRT = tutSepGO.GetComponent<RectTransform>();
        tutSepRT.anchorMin = new Vector2(0.5f, 1f);
        tutSepRT.anchorMax = new Vector2(0.5f, 1f);
        tutSepRT.pivot = new Vector2(0.5f, 1f);
        tutSepRT.sizeDelta = new Vector2(580f, 2f);
        tutSepRT.anchoredPosition = new Vector2(0f, -330f);

        // ── Leyenda de fichas ────────────────────────────────────────────────
        string[] chipNames = { "Energía", "Personas", "Reputación", "Dinero" };
        string[] chipFieldNames = {
            "chipIconEnergy", "chipIconPeople", "chipIconReputation", "chipIconMoney"
        };
        Image[] legendIcons = new Image[4];

        for (int i = 0; i < 4; i++)
        {
            float rowY = -360f - i * 130f;

            var rowGO = CreateUIObject($"ChipRow{i}", tutCard.transform);
            var rowRT = rowGO.GetComponent<RectTransform>();
            rowRT.anchorMin = new Vector2(0.5f, 1f);
            rowRT.anchorMax = new Vector2(0.5f, 1f);
            rowRT.pivot = new Vector2(0.5f, 1f);
            rowRT.sizeDelta = new Vector2(560f, 110f);
            rowRT.anchoredPosition = new Vector2(0f, rowY);

            // Icono de la ficha
            var iconGO = CreateUIObject("ChipIcon", rowGO.transform);
            var iconImg = iconGO.AddComponent<Image>();
            iconImg.color = new Color(1f, 1f, 1f, 0.9f);
            legendIcons[i] = iconImg;
            var iconRT = iconGO.GetComponent<RectTransform>();
            iconRT.anchorMin = new Vector2(0f, 0.5f);
            iconRT.anchorMax = new Vector2(0f, 0.5f);
            iconRT.pivot = new Vector2(0f, 0.5f);
            iconRT.sizeDelta = new Vector2(80f, 80f);
            iconRT.anchoredPosition = new Vector2(0f, 0f);

            // Nombre de la ficha
            var nameGO = CreateUIObject("ChipName", rowGO.transform);
            var nameTMP = nameGO.AddComponent<TextMeshProUGUI>();
            nameTMP.text = chipNames[i];
            nameTMP.fontSize = 48;
            nameTMP.fontStyle = FontStyles.Bold;
            nameTMP.alignment = TextAlignmentOptions.MidlineLeft;
            nameTMP.color = Color.white;
            var nameRT = nameGO.GetComponent<RectTransform>();
            nameRT.anchorMin = new Vector2(0f, 0f);
            nameRT.anchorMax = new Vector2(1f, 1f);
            nameRT.offsetMin = new Vector2(105f, 0f);
            nameRT.offsetMax = new Vector2(0f, 0f);
        }

        // Botón OK
        var okBtn = CreateButton("OkButton", tutCard.transform, "OK",
            new Color(0.2f, 0.6f, 1f, 1f), -920f, 100f);

        // ════════════════════════════════════════════════════════════════════
        // PANEL GESTURE — Panel 2: imágenes de gestos (flechas + mano)
        // ════════════════════════════════════════════════════════════════════
        var gesturePanel = CreateUIObject("GesturePanel", canvasGO.transform);
        var gesturePanelImg = gesturePanel.AddComponent<Image>();
        gesturePanelImg.color = Color.clear;
        StretchFull(gesturePanel.GetComponent<RectTransform>());
        gesturePanel.SetActive(false);

        // Flecha izquierda
        var arrowLeftGO = CreateUIObject("ArrowLeft", gesturePanel.transform);
        var arrowLeftImg = arrowLeftGO.AddComponent<Image>();
        arrowLeftImg.color = Color.white;
        var arrowLeftRT = arrowLeftGO.GetComponent<RectTransform>();
        arrowLeftRT.anchorMin = new Vector2(0.5f, 0.5f);
        arrowLeftRT.anchorMax = new Vector2(0.5f, 0.5f);
        arrowLeftRT.pivot = new Vector2(0.5f, 0.5f);
        arrowLeftRT.sizeDelta = new Vector2(140f, 140f);
        arrowLeftRT.anchoredPosition = new Vector2(-300f, 0f);

        // Flecha derecha
        var arrowRightGO = CreateUIObject("ArrowRight", gesturePanel.transform);
        var arrowRightImg = arrowRightGO.AddComponent<Image>();
        arrowRightImg.color = Color.white;
        var arrowRightRT = arrowRightGO.GetComponent<RectTransform>();
        arrowRightRT.anchorMin = new Vector2(0.5f, 0.5f);
        arrowRightRT.anchorMax = new Vector2(0.5f, 0.5f);
        arrowRightRT.pivot = new Vector2(0.5f, 0.5f);
        arrowRightRT.sizeDelta = new Vector2(140f, 140f);
        arrowRightRT.anchoredPosition = new Vector2(300f, 0f);

        // Mano apuntando a la carta
        var handGO = CreateUIObject("HandPointer", gesturePanel.transform);
        var handImg = handGO.AddComponent<Image>();
        handImg.color = Color.white;
        var handRT = handGO.GetComponent<RectTransform>();
        handRT.anchorMin = new Vector2(0.5f, 0.5f);
        handRT.anchorMax = new Vector2(0.5f, 0.5f);
        handRT.pivot = new Vector2(0.5f, 0.5f);
        handRT.sizeDelta = new Vector2(140f, 140f);
        handRT.anchoredPosition = new Vector2(0f, -200f);

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
        so.FindProperty("gameRulesToggle").objectReferenceValue   = gameRulesToggle;
        so.FindProperty("gameRulesLabel").objectReferenceValue    = gameRulesLabelTMP;
        so.FindProperty("registerButton").objectReferenceValue    = registerBtn;
        so.FindProperty("feedbackText").objectReferenceValue      = feedbackTMP;
        so.FindProperty("playerScoreText").objectReferenceValue    = playerScoreTMP;
        so.FindProperty("playerBestText").objectReferenceValue    = playerBestTMP;
        so.FindProperty("leaderboardStatus").objectReferenceValue = lbStatusTMP;
        so.FindProperty("leaderboardLeft").objectReferenceValue   = lbLeftTMP;
        so.FindProperty("leaderboardRight").objectReferenceValue  = lbRightTMP;
        so.FindProperty("retryButton").objectReferenceValue       = retryBtn;
        so.ApplyModifiedProperties();

        // ── Conectar TutorialManager ─────────────────────────────────────────
        var tmGO = GameObject.Find("TutorialManager") ?? new GameObject("TutorialManager");
        var tm = tmGO.GetComponent<TutorialManager>() ?? tmGO.AddComponent<TutorialManager>();

        var soTM = new SerializedObject(tm);
        soTM.FindProperty("tutorialPanel").objectReferenceValue = tutorialPanel;
        soTM.FindProperty("gesturePanel").objectReferenceValue  = gesturePanel;
        soTM.ApplyModifiedProperties();

        // Conectar botones y toggles
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            registerBtn.onClick, lm.OnRegisterPressed);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            retryBtn.onClick, lm.OnRetryPressed);
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            privacyToggle.onValueChanged, lm.OnPrivacyToggleChanged, false);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            privacyLinkBtn.onClick, lm.OpenPrivacyLink);
        UnityEditor.Events.UnityEventTools.AddBoolPersistentListener(
            gameRulesToggle.onValueChanged, lm.OnGameRulesToggleChanged, false);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            gameRulesLinkBtn.onClick, lm.OpenGameRulesLink);
        UnityEditor.Events.UnityEventTools.AddVoidPersistentListener(
            okBtn.onClick, tm.OnOkPressed);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Selection.activeGameObject = canvasGO;
        Debug.Log("✓ LoginCanvas con Login, Highscore y Tutorial creado. Guarda con Ctrl+S.");
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

    private static (Toggle toggle, TextMeshProUGUI label, Button linkButton) CreateCheckboxRow(
        string rowName, Transform parent, string labelText, string linkText, float yPos)
    {
        // Contenedor de la fila
        var rowGO = CreateUIObject(rowName, parent);
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

        // ── Texto label ──────────────────────────────────────────────────────
        var labelGO = CreateUIObject("Label", rowGO.transform);
        var labelTMP = labelGO.AddComponent<TextMeshProUGUI>();
        labelTMP.text = labelText;
        labelTMP.fontSize = 33;
        labelTMP.color = new Color(0.8f, 0.8f, 0.8f, 1f);
        labelTMP.alignment = TextAlignmentOptions.MidlineLeft;
        var labelRT = labelGO.GetComponent<RectTransform>();
        labelRT.anchorMin = new Vector2(0f, 0f);
        labelRT.anchorMax = new Vector2(0f, 1f);
        labelRT.pivot = new Vector2(0f, 0.5f);
        labelRT.sizeDelta = new Vector2(350f, 0f);
        labelRT.anchoredPosition = new Vector2(70f, 0f);

        // ── Botón-link ───────────────────────────────────────────────────────
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
        linkTMP.text = $"<u>{linkText}</u>";
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
