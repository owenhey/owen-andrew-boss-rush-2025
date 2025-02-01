using System;
using UnityEngine;

public class RagdollHelper : MonoBehaviour {
    public Rigidbody rb;
    public new Collider collider;

    void Reset() {
        rb = GetComponent<Rigidbody>();
        collider = GetComponent<Collider>();
    }

    public void Enable() {
        rb.isKinematic = false;
        collider.enabled = true;
        gameObject.layer = 14;
        transform.SetParent(null, true);

        rb.angularDamping = 5f;
        rb.linearDamping = .25f;
    }

    public void Disable() {
        rb.isKinematic = true;
        collider.enabled = false;
    }

    public void Push(Vector3 force) {
        Vector3 f = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f),
            UnityEngine.Random.Range(-1.0f, 1.0f)) * 3f;
        f += force * 7f;
        
        Vector3 r = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f),
            UnityEngine.Random.Range(-1.0f, 1.0f)) * 3f;
        rb.AddForce(f , ForceMode.Impulse);
        rb.AddRelativeTorque(r , ForceMode.Impulse);
    }
}
