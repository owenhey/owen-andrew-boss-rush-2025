using System;
using UnityEngine;

public class RobotCamera : MonoBehaviour {
    public GameObject robotCam;

    public GameObject robotEnv;
    public GameObject outsideEnv;

    public bool outsideCollider;

    private void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Movement m)) {
            if (outsideCollider) {
                robotCam.gameObject.SetActive(true);
                robotEnv.SetActive(true);
                outsideEnv.SetActive(false);
            }
            else {
                robotCam.gameObject.SetActive(false);
                robotEnv.SetActive(false);
                outsideEnv.SetActive(true);
            }
        }
    }
}
