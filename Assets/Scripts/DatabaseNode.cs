using UnityEngine;

public class DatabaseNode : BaseNode
{
    protected override string DefaultNodeName => "Database";
    protected override Color DefaultNodeColor => new Color(1f, 0.5f, 0f); // Orange

    protected override void OnRequestProcessed(NetworkRequest req)
    {
        // Database nodes process Read, Write, and Search requests
        if (req.Type == RequestType.Read || req.Type == RequestType.Write || req.Type == RequestType.Search)
        {
            Debug.Log($"[SUCCESS] Request {req.Id} ({req.Type}) processed by {NodeName}.");
            NetworkTelemetry.Instance?.RegisterSuccess(req.Type);
        }
        else if (req.Type == RequestType.Malicious)
        {
            Debug.LogWarning($"[BREACH] Malicious request {req.Id} breached {NodeName}!");
            NetworkTelemetry.Instance?.RegisterBreach();
        }
        else
        {
            // E.g., Static or Upload shouldn't go to DB
            Debug.LogWarning($"[DROP] {NodeName} cannot process {req.Type} traffic.");
            NetworkTelemetry.Instance?.RegisterDrop();
        }
    }
}
