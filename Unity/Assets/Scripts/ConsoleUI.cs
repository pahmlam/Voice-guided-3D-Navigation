using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConsoleUI : MonoBehaviour
{
    [Header("Auto")]
    public bool autoCreateAtRuntime = true;
    public bool captureUnityLogs = true;

    [Header("Layout")]
    public Vector2 panelSize = new Vector2(400, 100);
    public Vector2 panelOffset = new Vector2(20, -20);
    public int padding = 10;

    [Header("Text")]
    public int fontSize = 40;         
    public bool showOnlyLatest = true; //chỉ hiện lệnh mới nhất

    public TMP_Text consoleText;

    private Image background;
    private Outline outline;

    private static ConsoleUI _instance;
    private bool _handlingLog = false;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Boot()
    {
        var existing = Object.FindFirstObjectByType<ConsoleUI>();
        if (existing != null)
        {
            _instance = existing;
            return;
        }

        CreateRuntimeConsole();
    }

    private static void CreateRuntimeConsole()
    {
        var canvasGO = new GameObject(
            "RuntimeConsoleCanvas",
            typeof(Canvas),
            typeof(CanvasScaler),
            typeof(GraphicRaycaster)
        );

        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 9999;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;

        DontDestroyOnLoad(canvasGO);

        var panelGO = new GameObject(
            "RuntimeConsolePanel",
            typeof(RectTransform),
            typeof(Image),
            typeof(Outline)
        );
        panelGO.transform.SetParent(canvasGO.transform, false);

        var textGO = new GameObject(
            "RuntimeConsoleText",
            typeof(RectTransform),
            typeof(TextMeshProUGUI)
        );
        textGO.transform.SetParent(panelGO.transform, false);

        var tmp = textGO.GetComponent<TextMeshProUGUI>();
        tmp.text = "";
        tmp.color = Color.white;
        tmp.alignment = TextAlignmentOptions.TopLeft;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        tmp.overflowMode = TextOverflowModes.Overflow;

        var ui = panelGO.AddComponent<ConsoleUI>();
        ui.consoleText = tmp;

        _instance = ui;
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;

    
        if (autoCreateAtRuntime)
        {
            if (GetComponentInParent<Canvas>() == null || GetComponent<RectTransform>() == null)
            {
                CreateRuntimeConsole();
                Destroy(this);
                return;
            }
        }

        SetupUI();

        if (captureUnityLogs)
            Application.logMessageReceived += OnLogReceived;
    }

    private void OnDestroy()
    {
        if (_instance == this && captureUnityLogs)
            Application.logMessageReceived -= OnLogReceived;
    }

    private void SetupUI()
    {
        background = GetComponent<Image>();
        if (background == null) background = gameObject.AddComponent<Image>();
        background.color = new Color(0, 0, 0, 0.55f);

        outline = GetComponent<Outline>();
        if (outline == null) outline = gameObject.AddComponent<Outline>();
        outline.effectColor = Color.white;
        outline.effectDistance = new Vector2(2, -2);

        var panelRT = GetComponent<RectTransform>();
        panelRT.anchorMin = new Vector2(0, 1);
        panelRT.anchorMax = new Vector2(0, 1);
        panelRT.pivot = new Vector2(0, 1);
        panelRT.sizeDelta = panelSize;
        panelRT.anchoredPosition = panelOffset;

        if (consoleText == null)
            consoleText = GetComponentInChildren<TMP_Text>(true);

        var textRT = consoleText.rectTransform;
        textRT.anchorMin = Vector2.zero;
        textRT.anchorMax = Vector2.one;
        textRT.offsetMin = new Vector2(padding, padding);
        textRT.offsetMax = new Vector2(-padding, -padding);

        if (consoleText is TextMeshProUGUI tmp)
        {
            tmp.fontSize = fontSize; 
            tmp.color = Color.white;
            tmp.alignment = TextAlignmentOptions.TopLeft;
            tmp.textWrappingMode = TextWrappingModes.Normal;
            tmp.overflowMode = TextOverflowModes.Overflow;
        }
    }

    private void OnLogReceived(string condition, string stackTrace, LogType type)
    {
        if (_handlingLog) return;

        // Chặn spam TMP/font
        if (condition.Contains("TextMeshPro") ||
            condition.Contains("Unicode value") ||
            condition.Contains("font asset") ||
            condition.Contains("RuntimeConsoleText"))
            return;

        //Chỉ show log giọng nói
        bool isVoiceLog =
            condition.StartsWith("[VoiceCommand]") ||
            condition.Contains("Python>>") ||
            condition.Contains("Connected to Python") ||
            condition.Contains("Không kết nối được Python");

        if (!isVoiceLog) return;

        // Lấy phần lệnh cho gọn
        string msg = condition;
        if (msg.StartsWith("[VoiceCommand]"))
            msg = msg.Replace("[VoiceCommand]", "").Trim();
        if (msg.StartsWith("Python>>"))
            msg = msg.Replace("Python>>", "").Trim();

        _handlingLog = true;
        AddMessage(msg);
        _handlingLog = false;
    }

    public void AddMessage(string msg)
    {
        if (string.IsNullOrWhiteSpace(msg) || consoleText == null) return;

        if (showOnlyLatest)
            consoleText.text = msg;     
        else
            consoleText.text += msg + "\n";
    }
}
