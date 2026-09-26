using System.Collections.Generic;
using UnityEngine;

public class NetworkTelemetry : MonoBehaviour
{
    public static NetworkTelemetry Instance;

    [Header("Economy Settings")]
    public float Budget = 1000f;
    public float NodeUpkeepInterval = 5f;
    public float BreachPenalty = 25f;

    [Header("Reputation Settings")]
    public float Reputation = 100f;
    public float ReputationDropPenalty = 0.5f;
    public float ReputationBreachPenalty = 5f;
    public float ReputationRegenPerSecond = 0.5f;

    [Header("Live Metrics (Read-Only)")]
    public int TotalRequestsGenerated = 0;
    public int TotalRequestsSuccessful = 0;
    public int TotalRequestsDropped = 0;
    public int TotalBreaches = 0;
    public int TotalBlocked = 0;
    public float CurrentRPS = 0f;
    public float GoodputPercentage = 100f;

    private int _requestsThisSecond = 0;
    private float _rpsTimer = 0f;
    private float _upkeepTimer = 0f;

    private void Awake() => Instance = this;

    private void Update()
    {
        _rpsTimer += Time.deltaTime;
        if (_rpsTimer >= 1.0f)
        {
            CurrentRPS = _requestsThisSecond;
            _requestsThisSecond = 0;
            _rpsTimer = 0f;

            CalculateGoodput();
        }

        _upkeepTimer += Time.deltaTime;
        if (_upkeepTimer >= NodeUpkeepInterval)
        {
            _upkeepTimer = 0f;
            DeductNodeUpkeep();
        }

        ApplyReputationDelta(ReputationRegenPerSecond * Time.deltaTime);
    }

    public void RegisterRequestSpawned()
    {
        TotalRequestsGenerated++;
        _requestsThisSecond++;
    }

    public void RegisterSuccess(RequestType type = RequestType.Static)
    {
        TotalRequestsSuccessful++;
        Budget += RequestTypeInfo.GetReward(type);
    }

    public void RegisterDrop()
    {
        TotalRequestsDropped++;
        ApplyReputationDelta(-ReputationDropPenalty);
    }

    public void RegisterBreach()
    {
        TotalBreaches++;
        Budget -= BreachPenalty;
        ApplyReputationDelta(-ReputationBreachPenalty);
    }

    public void RegisterBlocked()
    {
        TotalBlocked++;
        // Intentionally no Budget/Reputation/Goodput impact — this is the
        // system working correctly, not a success or a failure event.
    }

    private void ApplyReputationDelta(float delta)
    {
        Reputation = Mathf.Clamp(Reputation + delta, 0f, 100f);
    }

    private void CalculateGoodput()
    {
        int totalProcessed = TotalRequestsSuccessful + TotalRequestsDropped + TotalBreaches;
        if (totalProcessed > 0)
        {
            GoodputPercentage = ((float)TotalRequestsSuccessful / totalProcessed) * 100f;
        }
        else
        {
            GoodputPercentage = 100f;
        }
    }

    private void DeductNodeUpkeep()
    {
        BaseNode[] allNodes = FindObjectsByType<BaseNode>(FindObjectsSortMode.None);
        float totalUpkeep = allNodes.Length * 5f;
        Budget -= totalUpkeep;
    }
}
