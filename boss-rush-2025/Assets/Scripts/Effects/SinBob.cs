using System;
using UnityEngine;

public class SinBob : MonoBehaviour {
    private Vector3 localOffset;

    public Vector3 amplitude;
    public float speed;

    private void Awake() {
        localOffset = transform.localPosition;
    }

    private void Update() {
        float t = (Mathf.Sin(speed * Time.time) + 1) / 2;
        transform.localPosition = localOffset + Vector3.Lerp(amplitude, -amplitude, t);
    }
}
