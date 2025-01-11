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
    public Vector3 LastMoveDirection => lastMoveDirection;

    public Transform targetPositionTrans;
    public Transform modelTrans;

    public ParticleSystem groundEffectPS;
    
    // Hoping
    [Header("Hopping")] 
    [Range(0, 3)]
    public float hopHeight = 1;
    [Range(0, 20)]
    public float hopTime = 3;

    [Header("Leaning")] 
    [Range(0, 10)] public float leanFactor = 1;
    
    // Input
    public InputActionReference moveAction;
    public InputActionReference rollAction;

    private Vector3 camForwardNoZ;

    public bool Attacking = false;
    

    private void OnEnable() {
        rollAction.action.started += Roll;
    }

    private void OnDisable() {
        rollAction.action.started -= Roll;
    }
    
    private void Roll(InputAction.CallbackContext obj) {
        TextPopups.Instance.Get().PopupAbove("Roll!", transform, 1.0f);
    }

    void Update() {
        camForwardNoZ = mainCam.transform.forward;
        camForwardNoZ.y = 0;
        
        Move();
    }

    private void Move() {
        Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
        Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);

        bool stopInput = Attacking;
        if (stopInput) {
            input = Vector3.zero;
        }
        
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
        Vector3 move = rotation * input * speed;

        targetPositionTrans.position = Vector3.SmoothDamp(targetPositionTrans.position, transform.position + move, ref _vel, damping);
        
        MoveTowards(targetPositionTrans.position);
        LeanTowards(targetPositionTrans.position);
    }

    public GameObject obj;
    
    private void MoveTowards(Vector3 hitpoint) {
        var position = playerCC.transform.position;
        Vector3 towards = hitpoint - position;

        // Update last movement direction if we're actually moving
        float magnitude = towards.magnitude;
        if (magnitude > 0.1f) {  // Small threshold to avoid jitter
            lastMoveDirection = towards.normalized;
            lastMoveDirection.y = 0;
        }
        Hop(magnitude);
    
        // Rotate towards the last movement direction
        Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
        playerCC.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20);
        towards.y = -1;
    
        playerCC.Move(towards * (speed * Time.deltaTime));
    }

    private void LeanTowards(Vector3 targetPos) {
        Vector3 towards = targetPos - playerCC.transform.position;
        towards.y = 0;
        float magnitude = towards.magnitude;

        Vector3 worldLean = Vector3.up + (towards * (magnitude * leanFactor));
        worldLean.Normalize();

        // Convert the world-space up vector to local space
        Vector3 localUp = playerCC.transform.InverseTransformDirection(Vector3.up);
        
        // Convert the target world direction to local space
        Vector3 localTargetDir = playerCC.transform.InverseTransformDirection(worldLean);
        
        // Calculate the rotation from localUp to localTargetDir
        Quaternion localRotation = Quaternion.FromToRotation(localUp, localTargetDir);

        modelTrans.localRotation = localRotation;
    }

    private float lastTimeHop = 0;
    private bool hopping = false;
    private float hopStop = -1;
    private bool stopped = true;
    private void Hop(float magnitude) {
        float t = Time.time - lastTimeHop;
        float tTimesHopTime = t * hopTime;
        float newY = Mathf.Abs(Mathf.Sin(tTimesHopTime)) * hopHeight;
        if (magnitude < .3f) {
            if (hopping) {
                hopStop = FindNextInterval(t, Mathf.PI / hopTime);
                hopping = false;
            }
            
            if (t < hopStop && !stopped) {
                modelTrans.localPosition = Vector3.up * newY;
            }
            else {
                stopped = true;
                lastTimeHop = Time.time;
                groundEffectPS.Stop();
            }
            return;
        }

        if (stopped) {
            groundEffectPS.Play();
        }

        hopping = true;
        stopped = false;
        modelTrans.localPosition = Vector3.up * newY;
    }
    
    private float FindNextInterval(float number, float interval)
    {
        float intervals = (float)Math.Ceiling(number / interval);
        float nextInterval = intervals * interval;
        
        // If the calculated interval equals the input number,
        // move to the next interval
        if (Math.Abs(nextInterval - number) < .01f) {
            return number;
        }
        
        return nextInterval;
    }
}
