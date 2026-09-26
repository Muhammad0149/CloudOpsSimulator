using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : MonoBehaviour
{
    [Header("Node Configuration")]
    public string NodeName = "Base Node";
    public Color NodeColor = Color.white;
    public int MaxQueueCapacity = 10;
    public float ProcessingRatePerSecond = 2f;
    public int CurrentQueueCount => IncomingQueue.Count;

    [Header("Connections")]
    public List<BaseNode> OutputNodes = new List<BaseNode>();

    protected Queue<NetworkRequest> IncomingQueue = new Queue<NetworkRequest>();
    protected float ProcessingTimer = 0f;

    // Visual Mesh Reference
    private Renderer _nodeRenderer;
    private MaterialPropertyBlock _propBlock;

    // Subclasses override this to say what they should be called by default.
    protected virtual string DefaultNodeName => "Base Node";
    protected virtual Color DefaultNodeColor => Color.white;

    protected virtual void Awake()
    {
        // Only replace the name if it's still the untouched base default —
        // anything typed in manually in the Inspector is left alone.
        if (string.IsNullOrWhiteSpace(NodeName) || NodeName == "Base Node")
        {
            NodeName = DefaultNodeName;
        }

        if (NodeColor == Color.white)
        {
            NodeColor = DefaultNodeColor;
        }

        _nodeRenderer = GetComponent<Renderer>();
        if (_nodeRenderer != null)
        {
            _propBlock = new MaterialPropertyBlock();
            _nodeRenderer.GetPropertyBlock(_propBlock);
            _propBlock.SetColor("_BaseColor", NodeColor);
            _propBlock.SetColor("_Color", NodeColor);
            _nodeRenderer.SetPropertyBlock(_propBlock);
        }
    }

    public virtual bool ReceiveRequest(NetworkRequest req)
    {
        if (IncomingQueue.Count >= MaxQueueCapacity)
        {
            Debug.LogWarning($"[DROP] {NodeName} queue full! Request {req.Id} dropped.");
            NetworkTelemetry.Instance?.RegisterDrop();
            return false;
        }

        IncomingQueue.Enqueue(req);
        return true;
    }

    protected virtual void Update()
    {
        ProcessQueue();
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
            Debug.Log($"[SUCCESS] Request {req.Id} ({req.Type}) completed at {NodeName}.");
            NetworkTelemetry.Instance?.RegisterSuccess(req.Type);
        }
    }

    protected virtual void ForwardRequest(NetworkRequest req)
    {
        foreach (var node in OutputNodes)
        {
            if (node.ReceiveRequest(req))
            {
                TrafficVisualizer.Instance?.SpawnPayloadVisual(transform.position, node.transform.position, req.Type);
                return;
            }
        }

        Debug.LogWarning($"[DROP] No downstream node could accept request {req.Id} from {NodeName}.");
        NetworkTelemetry.Instance?.RegisterDrop();
    }

    private void OnMouseEnter()
    {
        TooltipManager.Instance?.ShowTooltip(NodeName);
    }

    private void OnMouseExit()
    {
        TooltipManager.Instance?.HideTooltip();
    }
}
