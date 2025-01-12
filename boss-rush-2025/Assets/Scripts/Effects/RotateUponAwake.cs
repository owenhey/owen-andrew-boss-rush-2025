using UnityEngine;

public class RotateUponAwake : MonoBehaviour {
    public bool OnlyY = false;

    public void Awake() {
        if (OnlyY) {
            transform.localEulerAngles = new Vector3(0, Random.Range(0, 360), 0);
        }
        else {
            transform.localRotation = Random.rotation;
        }
    }
}
