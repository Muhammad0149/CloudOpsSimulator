using UnityEngine;

public class CacheNode : BaseNode
{
    protected override string DefaultNodeName => "Cache";
    protected override Color DefaultNodeColor => Color.magenta;

    [Header("Cache Settings")]
    [Range(0f, 1f)]
    public float CacheHitRate = 0.80f; // 80% of READ traffic is served from cache

    // We do not override ReceiveRequest, so all traffic enters the queue.

    protected override void OnRequestProcessed(NetworkRequest req)
    {
        if (req.Type == RequestType.Read)
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
                Debug.Log($"[CACHE MISS] {req.Id} forwarding downstream.");
            }
        }

        // Cache misses and non-read traffic pass through to downstream
        ForwardRequest(req);
    }
}
