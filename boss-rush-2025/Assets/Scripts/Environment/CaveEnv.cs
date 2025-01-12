using System.Collections.Generic;
using UnityEngine;

public class CaveEnv : MonoBehaviour {
    public List<Transform> swirlingRocks;

    public Transform caveCenter;
    public float rotateSpeed = 3;
    
    private void Awake() {
        foreach (var rock in swirlingRocks) {
            rock.transform.rotation = Random.rotation;
        }
    }

    private void Update() {
        caveCenter.transform.Rotate(0, Time.deltaTime * rotateSpeed, 0);
    }
}
