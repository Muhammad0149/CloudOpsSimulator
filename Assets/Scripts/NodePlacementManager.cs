using UnityEngine;
using UnityEngine.InputSystem;

public class NodePlacementManager : MonoBehaviour
{
    public GameObject ClientGeneratorPrefab; // Yellow Sphere
    public GameObject LoadBalancerPrefab;     // Blue Cylinder
    public GameObject ComputePrefab;          // Green Cube
    public GameObject FirewallPrefab;         // Red Block
    public GameObject CdnPrefab;              // CDN Node
    public GameObject StoragePrefab;          // Storage Node
    public GameObject DatabasePrefab;         // Database Node
    public GameObject CachePrefab;            // Cache Node

    public Material CableMaterial;

    private BaseNode _selectedSourceNode;

    void Update()
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.digit1Key.wasPressedThisFrame) SpawnNodeAtMouse(ClientGeneratorPrefab);
            if (Keyboard.current.digit2Key.wasPressedThisFrame) SpawnNodeAtMouse(LoadBalancerPrefab);
            if (Keyboard.current.digit3Key.wasPressedThisFrame) SpawnNodeAtMouse(ComputePrefab);
            if (Keyboard.current.digit4Key.wasPressedThisFrame) SpawnNodeAtMouse(FirewallPrefab);
            if (Keyboard.current.digit5Key.wasPressedThisFrame) SpawnNodeAtMouse(CdnPrefab);
            if (Keyboard.current.digit6Key.wasPressedThisFrame) SpawnNodeAtMouse(StoragePrefab);
            if (Keyboard.current.digit7Key.wasPressedThisFrame) SpawnNodeAtMouse(DatabasePrefab);
            if (Keyboard.current.digit8Key.wasPressedThisFrame) SpawnNodeAtMouse(CachePrefab);
        }

        if (Mouse.current != null)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();
                Ray ray = Camera.main.ScreenPointToRay(mousePos);

                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    BaseNode hitNode = hit.collider.GetComponent<BaseNode>();
                    if (hitNode != null)
                    {
                        if (_selectedSourceNode == null)
                        {
                            _selectedSourceNode = hitNode;
                            Debug.Log($"Selected Source: {_selectedSourceNode.NodeName}");
                        }
                        else if (_selectedSourceNode != hitNode)
                        {
                            _selectedSourceNode.OutputNodes.Add(hitNode);
                            CreateCableVisual(_selectedSourceNode.transform.position, hitNode.transform.position);
                            Debug.Log($"Connected {_selectedSourceNode.NodeName} -> {hitNode.NodeName}");
                            _selectedSourceNode = null;
                        }
                    }
                }
            }

            if (Mouse.current.rightButton.wasPressedThisFrame)
            {
                _selectedSourceNode = null;
            }
        }
    }

    private void SpawnNodeAtMouse(GameObject prefab)
    {
        if (prefab == null)
        {
            Debug.LogWarning("Prefab missing in NodePlacementManager Inspector!");
            return;
        }

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 pos = new Vector3(Mathf.Round(hit.point.x), 0.5f, Mathf.Round(hit.point.z));
            Instantiate(prefab, pos, Quaternion.identity);
        }
    }

    private void CreateCableVisual(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("CableLink");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.startWidth = 0.08f;
        lr.endWidth = 0.08f;
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        if (CableMaterial != null) lr.material = CableMaterial;
    }
}