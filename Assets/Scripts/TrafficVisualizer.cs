using System.Collections;
using UnityEngine;

public class TrafficVisualizer : MonoBehaviour
{
    public static TrafficVisualizer Instance;
    public GameObject PayloadPrefab; // Small Red Sphere primitive scaled to (0.2, 0.2, 0.2)
    public float TravelSpeed = 5f;

    private void Awake() => Instance = this;

    public void SpawnPayloadVisual(Vector3 start, Vector3 end)
    {
        if (PayloadPrefab == null) return;
        GameObject p = Instantiate(PayloadPrefab, start, Quaternion.identity);
        StartCoroutine(AnimatePayload(p, start, end));
    }

    private IEnumerator AnimatePayload(GameObject payload, Vector3 start, Vector3 end)
    {
        float progress = 0;
        while (progress < 1f)
        {
            progress += Time.deltaTime * TravelSpeed;
            payload.transform.position = Vector3.Lerp(start, end, progress);
            yield return null;
        }
        Destroy(payload);
    }
}