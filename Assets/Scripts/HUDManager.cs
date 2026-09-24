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
    public float Spacing = 60f;

    [Header("Stats To Display (add/reorder freely)")]
    public List<HudStatEntry> Stats = new List<HudStatEntry>
    {
        new HudStatEntry { Label = "Budget", Type = HudStatType.Budget },
        new HudStatEntry { Label = "RPS", Type = HudStatType.RPS },
        new HudStatEntry { Label = "Goodput", Type = HudStatType.Goodput },
        new HudStatEntry { Label = "Reputation", Type = HudStatType.Reputation },
        new HudStatEntry { Label = "Drops", Type = HudStatType.Drops },
        new HudStatEntry { Label = "Breaches", Type = HudStatType.Breaches },
    };

    private const string LabelColor = "#A9A9A9";
    private const string GoodColor = "#7CFC9E";
    private const string WarnColor = "#FFD166";
    private const string BadColor = "#FF6B6B";
    private const string RpsColor = "#7EC8FF";
    private const string DropsColor = "#FFB347";

    private readonly List<TMP_Text> _instances = new List<TMP_Text>();

    private void Start()
    {
        if (HudContainer == null || StatTextTemplate == null)
        {
            Debug.LogWarning("HUDManager is missing HudContainer or StatTextTemplate.");
            return;
        }

        foreach (var stat in Stats)
        {
            TMP_Text instance = Instantiate(StatTextTemplate, HudContainer);
            instance.gameObject.SetActive(true);
            instance.gameObject.name = $"Stat_{stat.Type}";
            instance.richText = true;
            instance.textWrappingMode = TextWrappingModes.NoWrap;
            instance.overflowMode = TextOverflowModes.Overflow;

            RectTransform rt = instance.rectTransform;
            rt.anchorMin = new Vector2(0f, 0.5f);
            rt.anchorMax = new Vector2(0f, 0.5f);
            rt.pivot = new Vector2(0f, 0.5f);

            _instances.Add(instance);
        }
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
                    return $"<color={LabelColor}>{stat.Label}:</color> <color={color}>${t.Budget:F0}</color>";
                }
            case HudStatType.RPS:
                return $"<color={LabelColor}>{stat.Label}:</color> <color={RpsColor}>{t.CurrentRPS}</color>";

            case HudStatType.Goodput:
                {
                    string color = t.GoodputPercentage >= 80f ? GoodColor
                                  : t.GoodputPercentage >= 50f ? WarnColor
                                  : BadColor;
                    return $"<color={LabelColor}>{stat.Label}:</color> <color={color}>{t.GoodputPercentage:F1}%</color>";
                }
            case HudStatType.Reputation:
                {
                    string color = t.Reputation >= 80f ? GoodColor
                                  : t.Reputation >= 50f ? WarnColor
                                  : BadColor;
                    return $"<color={LabelColor}>{stat.Label}:</color> <color={color}>{t.Reputation:F0}%</color>";
                }
            case HudStatType.Drops:
                return $"<color={LabelColor}>{stat.Label}:</color> <color={DropsColor}>{t.TotalRequestsDropped}</color>";

            case HudStatType.Breaches:
                {
                    string color = t.TotalBreaches > 0 ? BadColor : LabelColor;
                    return $"<color={LabelColor}>{stat.Label}:</color> <color={color}>{t.TotalBreaches}</color>";
                }
            default:
                return $"{stat.Label}: ?";
        }
    }
}