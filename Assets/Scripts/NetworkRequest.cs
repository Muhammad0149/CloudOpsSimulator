using UnityEngine;

/// <summary>
/// All traffic types in the simulation, matching the Server Survival model.
/// Each type has a designated destination category and reward value.
/// </summary>
public enum RequestType
{
    Static,     // Green  — CDN / Storage
    Read,       // Blue   — Database / Cache / NoSQL
    Write,      // Orange — Database / NoSQL
    Upload,     // Yellow — Storage
    Search,     // Cyan   — Search Engine / Database
    Malicious,  // Red    — Blocked by Firewall / Identity Provider
    Inference   // Fuchsia — GPU Cluster (via Inference Gateway)
}

/// <summary>
/// Lookup table for per-type reward values and visual colors.
/// Based on Server Survival's traffic reward model.
/// </summary>
public static class RequestTypeInfo
{
    /// <summary>
    /// Revenue earned when a request of this type is successfully processed.
    /// </summary>
    public static float GetReward(RequestType type)
    {
        switch (type)
        {
            case RequestType.Static:    return 0.50f;
            case RequestType.Read:      return 0.80f;
            case RequestType.Write:     return 1.20f;
            case RequestType.Upload:    return 1.50f;
            case RequestType.Search:    return 1.20f;
            case RequestType.Malicious: return 0.00f;
            case RequestType.Inference: return 0.50f;
            default:                    return 0.50f;
        }
    }

    /// <summary>
    /// Visual color for traffic visualizer payloads.
    /// </summary>
    public static Color GetColor(RequestType type)
    {
        switch (type)
        {
            case RequestType.Static:    return Color.green;
            case RequestType.Read:      return new Color(0.3f, 0.5f, 1.0f);   // Blue
            case RequestType.Write:     return new Color(1.0f, 0.6f, 0.2f);   // Orange
            case RequestType.Upload:    return Color.yellow;
            case RequestType.Search:    return Color.cyan;
            case RequestType.Malicious: return Color.red;
            case RequestType.Inference: return new Color(1.0f, 0.0f, 1.0f);   // Fuchsia
            default:                    return Color.white;
        }
    }
}

public class NetworkRequest
{
    public string Id;
    public RequestType Type;
    public float SpawnTime;
    public float DeadLine = 5.0f;


    public NetworkRequest(RequestType type = RequestType.Static)
    {
        Id = System.Guid.NewGuid().ToString();
        Type = type;
        SpawnTime = Time.time;
    }
}
