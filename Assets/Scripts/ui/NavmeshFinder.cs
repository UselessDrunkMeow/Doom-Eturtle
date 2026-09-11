using UnityEngine;
using Unity.AI.Navigation;

public class NavMeshSurfaceFinder : MonoBehaviour
{
    void Start()
    {
        var surfaces = FindObjectsByType<NavMeshSurface>(FindObjectsSortMode.None);

        Debug.Log($"Aantal NavMeshSurfaces gevonden: {surfaces.Length}");

        foreach (var surface in surfaces)
        {
            Debug.Log($"NavMeshSurface: {surface.gameObject.name}");
        }
    }
}
