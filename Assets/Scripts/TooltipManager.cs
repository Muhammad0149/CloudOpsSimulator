using UnityEngine;
using TMPro;

public class TooltipManager : MonoBehaviour
{
    public static TooltipManager Instance;

    public TextMeshProUGUI TooltipText;
    public GameObject TooltipPanel;

    private void Awake()
    {
        Instance = this;
        if (TooltipText != null)
        {
            TooltipText.fontSize = 20f;
            TooltipText.alignment = TextAlignmentOptions.Center;
            TooltipText.textWrappingMode = TextWrappingModes.NoWrap;
        }

        if (TooltipPanel != null && TooltipPanel.TryGetComponent<RectTransform>(out var rt))
        {
            rt.sizeDelta = new Vector2(320f, 75f);
        }

        HideTooltip();
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Mouse.current == null) return;

        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();
        
        // 1. Position the tooltip offset from the cursor
        if (TooltipPanel != null && TooltipPanel.activeSelf)
        {
            TooltipPanel.transform.position = mousePos + new Vector2(24f, -24f);
        }

        // 2. Raycast to detect nodes under the mouse (bypassing OnMouseEnter issues with New Input System)
        Ray ray = Camera.main.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            BaseNode hitNode = hit.collider.GetComponent<BaseNode>();
            if (hitNode != null)
            {
                string info = $"<color=#38BDF8><b>{hitNode.NodeName}</b></color>\n<color=#94A3B8>Queue:</color> <b>{hitNode.CurrentQueueCount}/{hitNode.MaxQueueCapacity}</b>   <color=#334155>|</color>   <color=#94A3B8>Rate:</color> <b>{hitNode.ProcessingRatePerSecond:F1}/s</b>";
                ShowTooltip(info);
                return; // We found a node, so stop here
            }
        }
        
        // If the raycast didn't hit a node, hide the tooltip
        HideTooltip();
    }

    public void ShowTooltip(string text)
    {
        if (TooltipText != null && TooltipPanel != null)
        {
            TooltipText.text = text;
            TooltipPanel.SetActive(true);
        }
    }

    public void HideTooltip()
    {
        if (TooltipPanel != null)
        {
            TooltipPanel.SetActive(false);
        }
    }
}
