using UnityEngine;

public class PlaneTest : MonoBehaviour
{
    MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sortingOrder = -2; 
    }
}
