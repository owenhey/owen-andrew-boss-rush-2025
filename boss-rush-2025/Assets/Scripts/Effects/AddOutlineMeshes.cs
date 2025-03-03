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
            if (renderer.gameObject.layer == 10) {
                renderer.enabled = false;
                continue;
            }
            if (renderer.gameObject.layer == 11) {
                continue;
            }

            if (renderer.CompareTag("dontoutline")) {
                continue;
            }
            
            var newObj = Instantiate(renderer.gameObject, renderer.transform);
            var newRenderer = newObj.GetComponent<MeshRenderer>();
            newRenderer.sharedMaterial = objMat;
            newObj.layer = 3;
            newRenderer.transform.SetLocalPositionAndRotation(Vector3.zero, quaternion.identity);
            newRenderer.transform.localScale = Vector3.one;
            newRenderer.shadowCastingMode = ShadowCastingMode.Off;
            
            if (renderer.gameObject.layer == 12) {
                Destroy(newRenderer.GetComponent<Rigidbody>());
                Destroy(newRenderer.GetComponent<Collider>());
            }
            
            if (newRenderer.TryGetComponent(out Collider c)) {
                Destroy(c);
            }
            if (newRenderer.TryGetComponent(out Rigidbody rb)) {
                Destroy(rb);
            }
            if (newRenderer.TryGetComponent(out RagdollHelper rh)) {
                Destroy(rh);
            }
            if (newRenderer.TryGetComponent(out RotateNearby rn)) {
                Destroy(rn);
            }
            if (newRenderer.TryGetComponent(out SinBob s)) {
                Destroy(s);
            }
            if (newRenderer.TryGetComponent(out RotateUponAwake r)) {
                Destroy(r);
            }
        }
    }
}
