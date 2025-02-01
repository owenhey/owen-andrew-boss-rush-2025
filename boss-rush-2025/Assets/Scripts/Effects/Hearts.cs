using UnityEngine;

public class Hearts : MonoBehaviour {
    public ParticleSystem ps;
    public Transform heartCenter;

    public Transform player;
    void Start() {
        ps.Stop();
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8) {
            ps.Play();
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8) {
            ps.Stop();
        }
    }

    private void Update() {
        float disToPlayer = (heartCenter.position - player.position).magnitude;

        disToPlayer /= 3.0f;
        disToPlayer = Mathf.Clamp01(disToPlayer);

        ps.transform.position = player.transform.position + Vector3.up * 2;

        var emission = ps.emission;
        emission.rateOverTime = 3 + (1 - disToPlayer) * 4;
    }
}
