using UnityEngine;

public enum RequestType { Standard, Malicious }

public class NetworkRequest
{
    public string Id;
    public RequestType Type;
    public float SpawnTime;
    public float DeadLine = 5.0f;


    public NetworkRequest(RequestType type =RequestType.Standard)
    {
        Id = System.Guid.NewGuid().ToString();
        Type = type;
        SpawnTime = Time.time;
    }
}
