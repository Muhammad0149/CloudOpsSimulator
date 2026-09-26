using System.Collections;
using UnityEngine;

public class TrafficVisualizer : MonoBehaviour
{
    public static TrafficVisualizer Instance;
    public GameObject PayloadPrefab; // Small Sphere primitive scaled to (0.2, 0.2, 0.2)
    public float TravelSpeed = 5f;

    private void Awake() => Instance = this;

    /// <summary>
    /// Spawns a color-coded payload visual between two nodes.
    /// Color is determined by the request type.
    /// </summary>
    public void SpawnPayloadVisual(Vector3 start, Vector3 end, RequestType type = RequestType.Static)
    {
        if (PayloadPrefab == null) return;
        GameObject p = Instantiate(PayloadPrefab, start, Quaternion.identity);

        // Apply type-specific color
        Renderer rend = p.GetComponent<Renderer>();
        if (rend != null)
        {
            rend.material.color = RequestTypeInfo.GetColor(type);
        }

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