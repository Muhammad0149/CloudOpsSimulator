using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : MonoBehaviour
{
    [Header("Node Configuration")]
    public string NodeName = "Base Node";
    public int MaxQueueCapacity = 10;
    public float ProcessingRatePerSecond = 2f;

    [Header("Connections")]
    public List<BaseNode> OutputNodes = new List<BaseNode>();

    protected Queue<NetworkRequest> IncomingQueue = new Queue<NetworkRequest>();
    protected float ProcessingTimer = 0f;

    // Visual Mesh Reference
    private Renderer _nodeRenderer;
    private MaterialPropertyBlock _propBlock;

    // Subclasses override this to say what they should be called by default.
    protected virtual string DefaultNodeName => "Base Node";

    protected virtual void Awake()
    {
        // Only replace the name if it's still the untouched base default —
        // anything typed in manually in the Inspector is left alone.
        if (string.IsNullOrWhiteSpace(NodeName) || NodeName == "Base Node")
        {
            NodeName = DefaultNodeName;
        }

        _nodeRenderer = GetComponent<Renderer>();
        _propBlock = new MaterialPropertyBlock();
    }

    public virtual bool ReceiveRequest(NetworkRequest req)
    {
        if (IncomingQueue.Count >= MaxQueueCapacity)
        {
            Debug.LogWarning($"[DROP] {NodeName} queue full! Request {req.Id} dropped.");
            NetworkTelemetry.Instance?.RegisterDrop();
            UpdateNodeColor();
            return false;
        }

        IncomingQueue.Enqueue(req);
        UpdateNodeColor();
        return true;
    }

    protected virtual void Update()
    {
        ProcessQueue();
        UpdateNodeColor();
    }

    protected virtual void ProcessQueue()
    {
        while (IncomingQueue.Count > 0 && IsExpired(IncomingQueue.Peek()))
        {
            NetworkRequest expired = IncomingQueue.Dequeue();
            Debug.LogWarning($"[TIMEOUT] Request {expired.Id} expired waiting at {NodeName}.");
            NetworkTelemetry.Instance?.RegisterDrop();
        }

        if (IncomingQueue.Count == 0) return;

        ProcessingTimer += Time.deltaTime;
        if (ProcessingTimer >= (1f / ProcessingRatePerSecond))
        {
            ProcessingTimer = 0f;
            NetworkRequest req = IncomingQueue.Dequeue();
            OnRequestProcessed(req);
        }
    }

    private bool IsExpired(NetworkRequest req)
    {
        return (Time.time - req.SpawnTime) >= req.DeadLine;
    }

    protected virtual void OnRequestProcessed(NetworkRequest req)
    {
        if (OutputNodes.Count > 0)
        {
            ForwardRequest(req);
        }
        else if (req.Type == RequestType.Malicious)
        {
            Debug.LogWarning($"[BREACH] Malicious request {req.Id} reached {NodeName} unfiltered!");
            NetworkTelemetry.Instance?.RegisterBreach();
        }
        else
        {
            Debug.Log($"[SUCCESS] Request {req.Id} completed at {NodeName}.");
            NetworkTelemetry.Instance?.RegisterSuccess();
        }
    }

    protected virtual void ForwardRequest(NetworkRequest req)
    {
        foreach (var node in OutputNodes)
        {
            if (node.ReceiveRequest(req))
            {
                TrafficVisualizer.Instance?.SpawnPayloadVisual(transform.position, node.transform.position);
                return;
            }
        }

        Debug.LogWarning($"[DROP] No downstream node could accept request {req.Id} from {NodeName}.");
        NetworkTelemetry.Instance?.RegisterDrop();
    }

    private static readonly int BaseColorID = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorID = Shader.PropertyToID("_Color");

    private void UpdateNodeColor()
    {
        if (_nodeRenderer == null) return;

        float fillRatio = (float)IncomingQueue.Count / MaxQueueCapacity;
        Color statusColor;

        if (fillRatio >= 0.85f)
        {
            statusColor = Color.red;
        }
        else if (fillRatio >= 0.50f)
        {
            statusColor = Color.yellow;
        }
        else
        {
            statusColor = Color.green;
        }

        _nodeRenderer.GetPropertyBlock(_propBlock);
        _propBlock.SetColor(BaseColorID, statusColor);
        _propBlock.SetColor(ColorID, statusColor);
        _nodeRenderer.SetPropertyBlock(_propBlock);

        if (_nodeRenderer.material != null && _nodeRenderer.material.HasProperty("_Color"))
        {
            _nodeRenderer.material.color = statusColor;
        }
    }
}