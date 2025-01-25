using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = System.Random;

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
    public Transform leg1transform;
    public Transform leg2transform;

    public ParticleSystem groundEffectPS;

    public LayerMask groundLayerMask;

    public PlayerAttacks playerAttacks;

    public static Vector3 GetPlayerPos() => _curPlayerTargetLoc.position;
    
    // Hoping
    [Header("Hopping")] 
    [Range(0, 3)]
    public float hopHeight = 1;
    [Range(0, 20)]
    public float hopTime = 3;
    [Range(0, 20)]
    public float legTime = 5;

    [Header("Leaning")] 
    [Range(0, 10)] public float leanFactor = 1;

    [Header("rolling")] [Range(0, 10)] public float rollDistance = .5f;
    [Range(0, 2)] public float rollTime = .5f;
    [Range(0, 1)] public float rollCooldown = .5f;

    private float nextRollAllowed = -1;
    private Vector3 rollTarget;
    private Vector3 rollStart;
    private float rollEndTime;
    private float rollStartTime;

    private static Transform _curPlayerTargetLoc;
    
    // Input
    public InputActionReference moveAction;
    public InputActionReference rollAction;

    private Vector3 camForwardNoZ;

    public bool Attacking = false;
    public bool Cutscened = false;

    private bool rolling = false;

    public Transform rollCenter;

    public FlailMovement flail;
    
    private void Awake() {
        targetPositionTrans.parent = null;
        _curPlayerTargetLoc = targetPositionTrans;

        Application.targetFrameRate = 144;
    }

    private void OnEnable() {
        rollAction.action.started += Roll;
    }

    private void OnDisable() {
        rollAction.action.started -= Roll;
    }

    public void ForceToFaceInputDirection() {
        Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
        if (inputV2 == Vector2.zero) return;
        
        Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);
        Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
        Vector3 direction = rotation * input;
        direction.y = 0;
        playerCC.transform.LookAt(playerCC.transform.position + direction);
    }

    public void ForceToFaceInputOrMouse() {
        if (DetectInputMode.IsController) {
            Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
            if (inputV2 == Vector2.zero) return;
        
            Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
            Vector3 direction = rotation * input;
            direction.y = 0;
            playerCC.transform.LookAt(playerCC.transform.position + direction);
        }
        else {
            // Raycast towards ground, if we hit, use the mouse, otherwise, just default to controller
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        
            if (false && Physics.Raycast(ray, out hit, 100, groundLayerMask)) {
                // Face towards this direction
                Vector3 towardsMouse = hit.point - playerCC.transform.position;
                towardsMouse.y = 0;
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
                Vector3 direction = rotation * towardsMouse;
                direction.y = 0;
                playerCC.transform.LookAt(playerCC.transform.position + direction);
            }
            else {
                // Just do the normal movemnet one
                Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
                if (inputV2 == Vector2.zero) return;
        
                Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);
                Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
                Vector3 direction = rotation * input;
                direction.y = 0;
                playerCC.transform.LookAt(playerCC.transform.position + direction);
            }
        }
    }
    
    private void Roll(InputAction.CallbackContext obj) {
        if (Attacking) return;
        if (rolling) return;
        if (Cutscened) return;
        if (gettingKnocked) return;

        gameObject.layer = 8; // No hit layer
        if (Time.time < nextRollAllowed) return;
        
        var playerPos = playerCC.transform.position;

        Vector3 direction;
        
        Vector2 inputV2 = moveAction.action.ReadValue<Vector2>();
        if (inputV2 != Vector2.zero) {
            Vector3 input = new Vector3(inputV2.x, 0, inputV2.y);
            Quaternion rotation = Quaternion.FromToRotation(Vector3.forward, camForwardNoZ);
            direction = rotation * input;
            ForceToFaceInputDirection();
        }
        else {
            direction = lastMoveDirection;
        }
        direction.Normalize();
        rolling = true;
        
        TextPopups.Instance.Get().PopupAbove("Roll!", transform.position, .25f);
        
        rollTarget = playerPos + direction * rollDistance;
        rollStart = playerPos;
        rollStartTime = Time.time;
        rollEndTime = Time.time + rollTime;
        nextRollAllowed = Time.time + rollTime + rollCooldown;

        targetPositionTrans.position = rollTarget + direction;
    }

    private bool gettingKnocked = false;
    private Vector3 knockPoint;
    public float knockbackForce;
    public float knockDuration = .25f;
    private float knockEnd = -1;
    private float knockStart;
    private Vector3 knockStartPoint;
    
    public void Knockback(Transform source) {
        TextPopups.Instance.Get().PopupAbove("Ouch!", transform, .5f);
        gettingKnocked = true;
        
        Vector3 towards = transform.position - source.position;
        towards.y = 0;
        towards.Normalize();

        knockEnd = Time.time + knockDuration;

        knockStart = Time.time;

        knockStartPoint = transform.position;
        knockPoint = transform.position + (knockbackForce * towards);
        targetPositionTrans.position = knockPoint;
    }

    void Update() {
        camForwardNoZ = mainCam.transform.forward;
        camForwardNoZ.y = 0;

        if (Input.GetKeyDown(KeyCode.G)) {
            BlowUp(Vector3.zero);
        }

        if (Cutscened) return;
        if (gettingKnocked) {
            if (Time.time > knockEnd) {
                gettingKnocked = false;
            }
            
            float t = (Time.time - knockStart) / knockDuration;
            t = Mathf.Sin((t * Mathf.PI) / 2);
            Vector3 target = Vector3.Lerp(knockStartPoint, knockPoint, t);
            Vector3 towardsKnockPoint = target - playerCC.transform.position;
            towardsKnockPoint.y = 0;
            playerCC.Move(towardsKnockPoint);
        }
        else {
            if (rolling) {
                RollTowards();
            }
            else {
                Move();
            }
        }
    }

    public void BlowUp(Vector3 forceDirection) {
        Cutscened = true;

        var rbs = GetComponentsInChildren<Rigidbody>(true);
        foreach (var rb in rbs) {
            rb.isKinematic = false;
            rb.transform.SetParent(null, true);
            rb.GetComponent<Collider>().enabled = true;
            Vector3 force = new Vector3(UnityEngine.Random.Range(-1.0f, 1.0f), UnityEngine.Random.Range(-1.0f, 1.0f),
                UnityEngine.Random.Range(-1.0f, 1.0f)) * .8f;
            force += (targetPositionTrans.position - transform.position) * 1.5f;
            force += (forceDirection) * 3.5f;
            rb.AddForce(force, ForceMode.Impulse);
        }
        
        groundEffectPS.Stop();

        flail.enabled = false;
    }

    private void RollTowards() {
        if (Time.time > rollEndTime) {
            rolling = false;
            rollCenter.localEulerAngles = Vector3.zero;
            rollCenter.localScale = Vector3.one;
            gameObject.layer = 7; // back to regular layer
            return;
        }

        float t = (Time.time - rollStartTime) / rollTime;
        t = 1 - Mathf.Pow(1 - (.5f * t), 3);
        t *= 1.144f;

        float scale = 2 * (t - .5f) * (t - .5f) + .5f;
        rollCenter.localScale = Vector3.one * Mathf.Sqrt(scale);
        
        Vector3 desired = Vector3.Lerp(rollStart, rollTarget, t);
        Vector3 direction = desired - playerCC.transform.position;
        direction.y = -1;
        playerCC.Move(direction);
        rollCenter.localEulerAngles = new Vector3(Mathf.Lerp(0, 360, t), 0, 0);
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

        if (playerAttacks.midSwing == false) {
            // Rotate towards the last movement direction
            Quaternion targetRotation = Quaternion.LookRotation(lastMoveDirection);
            playerCC.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20);
        }
        // Rotate towards the last movement direction
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
            leg1transform.localEulerAngles = Vector3.zero;
            leg2transform.localEulerAngles = Vector3.zero;
            
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

        leg1transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(t * legTime) * 20);
        leg2transform.localEulerAngles = new Vector3(0, 0, Mathf.Sin(t * legTime) * -20);
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
