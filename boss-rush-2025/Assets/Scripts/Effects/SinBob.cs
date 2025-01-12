using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class SinBob : MonoBehaviour {
    private Vector3 localOffset;

    public Vector3 amplitude;
    public float speed;

    private float offset;

    private void Awake() {
        localOffset = transform.localPosition;
        offset = Random.Range(0.0f, 10.0f);
    }

    private void Update() {
        float t = (Mathf.Sin(speed * (offset + Time.time)) + 1) / 2;
        transform.localPosition = localOffset + Vector3.Lerp(amplitude, -amplitude, t);
    }
}
