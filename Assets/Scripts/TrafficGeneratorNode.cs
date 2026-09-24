using UnityEngine;

public class TrafficGeneratorNode : BaseNode
{
    protected override string DefaultNodeName => "Traffic Generator";

    public float RequestsPerSecond = 1f;

    [Header("Threat Simulation")]
    [Range(0f, 1f)]
    public float MaliciousTrafficChance = 0.1f;

    private float _spawnTimer;

    protected override void Update()
    {
        base.Update();

        _spawnTimer += Time.deltaTime;
        if (_spawnTimer >= (1f / RequestsPerSecond))
        {
            _spawnTimer = 0f;
            GenerateTraffic();
        }
    }

    private void GenerateTraffic()
    {
        RequestType type = (Random.value < MaliciousTrafficChance) ? RequestType.Malicious : RequestType.Standard;
        NetworkRequest req = new NetworkRequest(type);

        NetworkTelemetry.Instance?.RegisterRequestSpawned();
        ForwardRequest(req);
    }
}