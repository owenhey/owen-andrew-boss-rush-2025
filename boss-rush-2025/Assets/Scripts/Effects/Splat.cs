using UnityEngine;

public class Splat : MonoBehaviour {
    [SerializeField] private ParticleSystem ps;

    public void Setup(Vector3 position, Color c) {
        gameObject.SetActive(true);
        var mainMod = ps.main;
        mainMod.startColor = c;
        ps.Play();
        ps.transform.position = position;
    }
}
