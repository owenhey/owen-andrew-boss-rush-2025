using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class AddOutlineMeshes : MonoBehaviour {
    public Material objMat;
    
    void Start() {
        GenerateMeshes();
    }

    private void GenerateMeshes() {
        var allRenderers = GameObject.FindObjectsByType<MeshRenderer>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var renderer in allRenderers) {
            if(renderer.GetComponent<TextMeshPro>()) continue;
            
            var newObj = Instantiate(renderer.gameObject, renderer.transform);
            var newRenderer = newObj.GetComponent<MeshRenderer>();
            newRenderer.sharedMaterial = objMat;
            newObj.layer = 3;
            newRenderer.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
            newRenderer.transform.localScale = Vector3.one;
            newRenderer.shadowCastingMode = ShadowCastingMode.Off;
            if (newRenderer.TryGetComponent(out Collider c)) {
                Destroy(c);
            }
        }
    }
}
