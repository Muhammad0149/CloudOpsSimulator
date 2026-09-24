using UnityEngine;

public class FirewallNode : BaseNode
{
    protected override string DefaultNodeName => "Firewall";

    [Header("Firewall Settings")]
    [Range(0f, 1f)]
    public float DetectionChance = 0.85f; // Chance a malicious request is caught here

    public override bool ReceiveRequest(NetworkRequest req)
    {
        if (req.Type == RequestType.Malicious && Random.value < DetectionChance)
        {
            Debug.Log($"[BLOCKED] {NodeName} filtered malicious request {req.Id}.");
            NetworkTelemetry.Instance?.RegisterBlocked();
            return true; // Absorbed here — never queued, never forwarded further
        }

        // Standard traffic, and any malicious traffic that slips past
        // detection, is queued and processed exactly like a normal node.
        return base.ReceiveRequest(req);
    }
}