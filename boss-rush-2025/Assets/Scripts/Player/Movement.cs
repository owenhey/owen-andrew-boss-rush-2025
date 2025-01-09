using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour {
    public CharacterController playerCC;
    public Camera mainCam;
    
    // Movement
    public float speed = 3;
    public float damping = .1f;
    private Vector3 _vel;
    private Vector3 lastMoveDirection = Vector3.forward;
    
    // Input
    public InputActionReference moveAction;
    public InputActionReference rollAction;

    private Vector3 camForwardNoZ;
    

    private void OnEnable() {
        rollAction.action.started += Roll;
    }

    private void OnDisable() {
        rollAction.action.started -= Roll;
    }
    
    private void Roll(InputAction.CallbackContext obj) {
        Debug.Log("Roll");
    }

    void Update() {
        camForwardNoZ = mainCam.transform.forward;
        camForwardNoZ.y = 0;

        Move();
    }

    private void Move() {
        Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
        Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);
        
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
        Vector3 move = rotation * input;

        MoveTowards(transform.position + move);
    }
    
    private void MoveTowards(Vector3 hitpoint) {
        Vector3 targetPos = Vector3.SmoothDamp(transform.position, hitpoint, ref _vel, damping, speed);
        Vector3 towards = targetPos - playerCC.transform.position;
    
        // Update last movement direction if we're actually moving
        if (towards.magnitude > 0.01f) {  // Small threshold to avoid jitter
            lastMoveDirection = towards.normalized;
            lastMoveDirection.y = 0;
        }
    
        // Rotate towards the last movement direction
        Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
        playerCC.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20);
    
        playerCC.Move(towards);
    }
}
