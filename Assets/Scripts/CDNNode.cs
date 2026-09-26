using UnityEngine;

public class CDNNode : BaseNode
{
    protected override string DefaultNodeName => "CDN";
    protected override Color DefaultNodeColor => Color.cyan;

    [Header("CDN Settings")]
    [Range(0f, 1f)]
    public float CacheHitRate = 0.95f; // 95% of STATIC traffic is served from cache

    // We do not override ReceiveRequest, so all traffic enters the queue.

    protected override void OnRequestProcessed(NetworkRequest req)
    {
        if (req.Type == RequestType.Static)
        {
            if (Random.value < CacheHitRate)
            {
                // Cache HIT! Successfully terminate the request here.
                Debug.Log($"[SUCCESS] Request {req.Id} (Cache Hit) completed at {NodeName}.");
                NetworkTelemetry.Instance?.RegisterSuccess(req.Type);
                return;
            }
            else
            {
                Debug.Log($"[CDN MISS] {req.Id} forwarding downstream.");
            }
        }

        // Cache misses and non-static traffic pass through to downstream
        ForwardRequest(req);
    }
}
