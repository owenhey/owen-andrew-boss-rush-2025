using UnityEngine;

public class Movement : MonoBehaviour {
    public Transform playerTrans;
    
    void Update() {
        var input = new Vector3(-Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        playerTrans.position += 8 * input * Time.deltaTime;
    }
}
