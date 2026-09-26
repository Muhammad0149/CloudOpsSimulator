using UnityEngine;

public class LoadBalancerNode : BaseNode
{
    protected override string DefaultNodeName => "Load Balancer";
    protected override Color DefaultNodeColor => Color.blue;

    private int _rrIndex = 0;

    protected override void ForwardRequest(NetworkRequest req)
    {
        if (OutputNodes.Count == 0)
        {
            Debug.LogWarning($"[DROP] Load Balancer {NodeName} has no output targets!");
            return;
        }

        BaseNode target = OutputNodes[_rrIndex % OutputNodes.Count];
        _rrIndex++;

        if (!target.ReceiveRequest(req))
        {
            Debug.LogWarning($"[DROP] Target {target.NodeName} refused request from LB.");
        }
        else
        {
            TrafficVisualizer.Instance?.SpawnPayloadVisual(transform.position, target.transform.position, req.Type);
        }
    }
}
