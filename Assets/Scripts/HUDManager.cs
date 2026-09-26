using System.Collections.Generic;
using UnityEngine;
using TMPro;

public enum HudStatType { Budget, RPS, Goodput, Reputation, Drops, Breaches }

[System.Serializable]
public class HudStatEntry
{
    public string Label = "Stat";
    public HudStatType Type;
}

public class HUDManager : MonoBehaviour
{
    [Header("Setup")]
    public RectTransform HudContainer;
    public TMP_Text StatTextTemplate;

    [Header("Layout")]
    public float Spacing = 36f;

    [Header("Optional Elements")]
    public TMP_Text TitleText;
    public RectTransform QuickbarContainer;
    public TMP_Text ControlsHintText;

    [Header("Stats To Display (add/reorder freely)")]
    public List<HudStatEntry> Stats = new List<HudStatEntry>
    {
        new HudStatEntry { Label = "BUDGET", Type = HudStatType.Budget },
        new HudStatEntry { Label = "TRAFFIC", Type = HudStatType.RPS },
        new HudStatEntry { Label = "GOODPUT", Type = HudStatType.Goodput },
        new HudStatEntry { Label = "REPUTATION", Type = HudStatType.Reputation },
        new HudStatEntry { Label = "DROPS", Type = HudStatType.Drops },
        new HudStatEntry { Label = "BREACHES", Type = HudStatType.Breaches },
    };

    private const string LabelColor = "#94A3B8";     // Slate 400
    private const string ValueDimColor = "#64748B";  // Slate 500
    private const string GoodColor = "#10B981";      // Emerald 500
    private const string WarnColor = "#F59E0B";      // Amber 500
    private const string BadColor = "#EF4444";       // Rose 500
    private const string RpsColor = "#38BDF8";       // Sky 400
    private const string DropsColor = "#FB923C";     // Orange 400
    private const string RepColor = "#06B6D4";       // Cyan 500

    private readonly List<TMP_Text> _instances = new List<TMP_Text>();

    private void Start()
    {
        if (HudContainer == null || StatTextTemplate == null)
        {
            Debug.LogWarning("HUDManager is missing HudContainer or StatTextTemplate.");
            return;
        }

        CreateHeaderBrand();

        foreach (var stat in Stats)
        {
            TMP_Text instance = Instantiate(StatTextTemplate, HudContainer);
            instance.gameObject.SetActive(true);
            instance.gameObject.name = $"Stat_{stat.Type}";
            instance.richText = true;
            instance.textWrappingMode = TextWrappingModes.NoWrap;
            instance.overflowMode = TextOverflowModes.Overflow;
            instance.fontSize = 15f;

            RectTransform rt = instance.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);

            _instances.Add(instance);
        }

        CreateBottomDock();
    }

    private void CreateHeaderBrand()
    {
        GameObject brandObj = new GameObject("BrandTitle", typeof(RectTransform));
        brandObj.transform.SetParent(HudContainer, false);
        RectTransform rt = brandObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0f, 0.5f);
        rt.anchorMax = new Vector2(0f, 0.5f);
        rt.pivot = new Vector2(0f, 0.5f);
        rt.anchoredPosition = new Vector2(24f, 0f);
        rt.sizeDelta = new Vector2(260f, 40f);

        TMP_Text title = Instantiate(StatTextTemplate, brandObj.transform);
        title.gameObject.SetActive(true);
        title.fontSize = 15f;
        title.alignment = TextAlignmentOptions.MidlineLeft;
        title.text = "<color=#38BDF8><b>CLOUDOPS</b></color> <color=#94A3B8>DEFENDER</color>  <color=#10B981><size=75%>● LIVE</size></color>";
        RectTransform trt = title.rectTransform;
        trt.anchorMin = Vector2.zero;
        trt.anchorMax = Vector2.one;
        trt.sizeDelta = Vector2.zero;
        trt.anchoredPosition = Vector2.zero;
    }

    private void CreateBottomDock()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        // Container for Build Dock
        GameObject dockObj = new GameObject("BuildDock", typeof(RectTransform), typeof(UnityEngine.UI.Image));
        dockObj.transform.SetParent(canvas.transform, false);

        RectTransform dockRt = dockObj.GetComponent<RectTransform>();
        dockRt.anchorMin = new Vector2(0.5f, 0f);
        dockRt.anchorMax = new Vector2(0.5f, 0f);
        dockRt.pivot = new Vector2(0.5f, 0f);
        dockRt.anchoredPosition = new Vector2(0f, 16f);
        dockRt.sizeDelta = new Vector2(960f, 48f);

        UnityEngine.UI.Image dockImg = dockObj.GetComponent<UnityEngine.UI.Image>();
        dockImg.color = new Color(0.043f, 0.059f, 0.098f, 0.90f);
        if (HudContainer != null && HudContainer.TryGetComponent<UnityEngine.UI.Image>(out var headerImg))
        {
            dockImg.sprite = headerImg.sprite;
            dockImg.type = UnityEngine.UI.Image.Type.Sliced;
        }

        var hlg = dockObj.AddComponent<UnityEngine.UI.HorizontalLayoutGroup>();
        hlg.spacing = 8f;
        hlg.padding = new RectOffset(10, 10, 6, 6);
        hlg.childAlignment = TextAnchor.MiddleCenter;
        hlg.childControlWidth = true;
        hlg.childControlHeight = true;
        hlg.childForceExpandWidth = true;
        hlg.childForceExpandHeight = true;

        (string key, string name, string hexColor)[] nodes = new[]
        {
            ("1", "Generator", "#FACC15"),
            ("2", "Load Balancer", "#38BDF8"),
            ("3", "Compute", "#4ADE80"),
            ("4", "Firewall", "#F87171"),
            ("5", "CDN", "#22D3EE"),
            ("6", "Storage", "#CBD5E1"),
            ("7", "Database", "#FB923C"),
            ("8", "Cache", "#E879F9")
        };

        foreach (var (key, name, hexColor) in nodes)
        {
            GameObject slotObj = new GameObject($"Slot_{key}", typeof(RectTransform), typeof(UnityEngine.UI.Image));
            slotObj.transform.SetParent(dockObj.transform, false);

            UnityEngine.UI.Image slotImg = slotObj.GetComponent<UnityEngine.UI.Image>();
            slotImg.color = new Color(0.08f, 0.12f, 0.20f, 0.85f);
            if (dockImg.sprite != null)
            {
                slotImg.sprite = dockImg.sprite;
                slotImg.type = UnityEngine.UI.Image.Type.Sliced;
            }

            TMP_Text slotText = Instantiate(StatTextTemplate, slotObj.transform);
            slotText.gameObject.SetActive(true);
            slotText.fontSize = 11.5f;
            slotText.alignment = TextAlignmentOptions.Center;
            slotText.text = $"<color={hexColor}><b>[{key}]</b></color> <color=#E2E8F0>{name}</color>";

            RectTransform textRt = slotText.rectTransform;
            textRt.anchorMin = Vector2.zero;
            textRt.anchorMax = Vector2.one;
            textRt.sizeDelta = Vector2.zero;
            textRt.anchoredPosition = Vector2.zero;
        }

        // Navigation hints in bottom-left
        GameObject hintObj = new GameObject("ControlsHint", typeof(RectTransform));
        hintObj.transform.SetParent(canvas.transform, false);
        RectTransform hintRt = hintObj.GetComponent<RectTransform>();
        hintRt.anchorMin = new Vector2(0f, 0f);
        hintRt.anchorMax = new Vector2(0f, 0f);
        hintRt.pivot = new Vector2(0f, 0f);
        hintRt.anchoredPosition = new Vector2(20f, 20f);
        hintRt.sizeDelta = new Vector2(380f, 30f);

        TMP_Text hintText = Instantiate(StatTextTemplate, hintObj.transform);
        hintText.gameObject.SetActive(true);
        hintText.fontSize = 11f;
        hintText.alignment = TextAlignmentOptions.Left;
        hintText.text = "<color=#64748B>[WASD]</color> Pan  <color=#334155>|</color>  <color=#64748B>[Scroll]</color> Zoom  <color=#334155>|</color>  <color=#64748B>[L-Click]</color> Connect  <color=#334155>|</color>  <color=#64748B>[R-Click]</color> Cancel";
        RectTransform htRt = hintText.rectTransform;
        htRt.anchorMin = Vector2.zero;
        htRt.anchorMax = Vector2.one;
        htRt.sizeDelta = Vector2.zero;
        htRt.anchoredPosition = Vector2.zero;
    }

    private void Update()
    {
        if (NetworkTelemetry.Instance == null) return;

        for (int i = 0; i < Stats.Count && i < _instances.Count; i++)
        {
            _instances[i].text = FormatStat(Stats[i]);
        }

        ArrangeHorizontally();
    }

    private void ArrangeHorizontally()
    {
        foreach (var text in _instances)
        {
            text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.rectTransform.sizeDelta.y);
        }

        float totalWidth = 0f;
        foreach (var text in _instances)
            totalWidth += text.preferredWidth;
        totalWidth += Spacing * (_instances.Count - 1);

        float x = (HudContainer.rect.width - totalWidth) / 2f;

        foreach (var text in _instances)
        {
            text.rectTransform.anchoredPosition = new Vector2(x, 0f);
            x += text.preferredWidth + Spacing;
        }
    }

    private string FormatStat(HudStatEntry stat)
    {
        NetworkTelemetry t = NetworkTelemetry.Instance;
        switch (stat.Type)
        {
            case HudStatType.Budget:
                {
                    string color = t.Budget >= 0 ? GoodColor : BadColor;
                    return $"<color={LabelColor}>{stat.Label}</color>  <color={color}><b>${t.Budget:N0}</b></color>";
                }
            case HudStatType.RPS:
                return $"<color={LabelColor}>{stat.Label}</color>  <color={RpsColor}><b>{t.CurrentRPS:F0} <size=75%>RPS</size></b></color>";

            case HudStatType.Goodput:
                {
                    string color = t.GoodputPercentage >= 80f ? GoodColor
                                  : t.GoodputPercentage >= 50f ? WarnColor
                                  : BadColor;
                    return $"<color={LabelColor}>{stat.Label}</color>  <color={color}><b>{t.GoodputPercentage:F1}%</b></color>";
                }
            case HudStatType.Reputation:
                {
                    string color = t.Reputation >= 80f ? RepColor
                                  : t.Reputation >= 50f ? WarnColor
                                  : BadColor;
                    return $"<color={LabelColor}>{stat.Label}</color>  <color={color}><b>{t.Reputation:F0}%</b></color>";
                }
            case HudStatType.Drops:
                {
                    string color = t.TotalRequestsDropped > 0 ? DropsColor : ValueDimColor;
                    return $"<color={LabelColor}>{stat.Label}</color>  <color={color}><b>{t.TotalRequestsDropped}</b></color>";
                }
            case HudStatType.Breaches:
                {
                    string color = t.TotalBreaches > 0 ? BadColor : ValueDimColor;
                    return $"<color={LabelColor}>{stat.Label}</color>  <color={color}><b>{t.TotalBreaches}</b></color>";
                }
            default:
                return $"{stat.Label}: ?";
        }
    }
}