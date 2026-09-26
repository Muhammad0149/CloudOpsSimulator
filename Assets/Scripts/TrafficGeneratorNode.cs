using UnityEngine;

public class TrafficGeneratorNode : BaseNode
{
    protected override string DefaultNodeName => "Traffic Generator";
    protected override Color DefaultNodeColor => Color.yellow;

    public float RequestsPerSecond = 1f;

    [Header("Traffic Distribution (must sum to 1.0)")]
    [Range(0f, 1f)] public float StaticChance    = 0.30f;
    [Range(0f, 1f)] public float ReadChance       = 0.20f;
    [Range(0f, 1f)] public float WriteChance      = 0.15f;
    [Range(0f, 1f)] public float UploadChance     = 0.05f;
    [Range(0f, 1f)] public float SearchChance     = 0.10f;
    [Range(0f, 1f)] public float MaliciousChance  = 0.20f;
    // Inference gets the remainder (defaults to 0.00)

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
        RequestType type = RollRequestType();
        NetworkRequest req = new NetworkRequest(type);

        NetworkTelemetry.Instance?.RegisterRequestSpawned();
        ForwardRequest(req);
    }

    /// <summary>
    /// Picks a RequestType using weighted random selection based on
    /// the Inspector-configured distribution sliders.
    /// </summary>
    private RequestType RollRequestType()
    {
        float roll = Random.value;
        float cumulative = 0f;

        cumulative += StaticChance;
        if (roll < cumulative) return RequestType.Static;

        cumulative += ReadChance;
        if (roll < cumulative) return RequestType.Read;

        cumulative += WriteChance;
        if (roll < cumulative) return RequestType.Write;

        cumulative += UploadChance;
        if (roll < cumulative) return RequestType.Upload;

        cumulative += SearchChance;
        if (roll < cumulative) return RequestType.Search;

        cumulative += MaliciousChance;
        if (roll < cumulative) return RequestType.Malicious;

        // Remainder goes to Inference
        return RequestType.Inference;
    }
}