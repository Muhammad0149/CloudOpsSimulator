using UnityEngine;

public class StorageNode : BaseNode
{
    protected override string DefaultNodeName => "Storage";
    protected override Color DefaultNodeColor => new Color(0.4f, 0.4f, 0.5f); // Slate Gray

    protected override void OnRequestProcessed(NetworkRequest req)
    {
        // Storage nodes terminate Static and Upload requests
        if (req.Type == RequestType.Static || req.Type == RequestType.Upload)
        {
            Debug.Log($"[SUCCESS] Request {req.Id} ({req.Type}) stored at {NodeName}.");
            NetworkTelemetry.Instance?.RegisterSuccess(req.Type);
        }
        else if (req.Type == RequestType.Malicious)
        {
            Debug.LogWarning($"[BREACH] Malicious request {req.Id} breached {NodeName}!");
            NetworkTelemetry.Instance?.RegisterBreach();
        }
        else
        {
            // Invalid traffic type for this node
            Debug.LogWarning($"[DROP] {NodeName} cannot process {req.Type} traffic.");
            NetworkTelemetry.Instance?.RegisterDrop();
        }
    }
}
