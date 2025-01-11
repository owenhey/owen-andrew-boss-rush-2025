using UnityEngine;

public class FlailMovement : MonoBehaviour {

    public float flailSpeed;
    
    public Transform flail;
    
    
    
    
    // Update is called once per frame
    void Update() {
        flail.localEulerAngles = new Vector3(0, Time.time * flailSpeed, 0);
    }
}
