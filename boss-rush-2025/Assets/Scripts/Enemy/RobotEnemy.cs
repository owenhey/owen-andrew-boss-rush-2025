using System.Collections;
using UnityEngine;

public class RobotEnemy : Enemy {
    private float nextMove = float.MaxValue;
    private float nextAttack = float.MaxValue;
    private bool attacking = false;
    private Vector3 attackTargetVel;

    [Header("Stats")] 
    public float leanFactor = .08f;
    public float firstMoveTime = 2.0f;
    public float firstAttackTime = 3.5f;
    public float timeBetweenMoves = 5.0f;
    public float timeBetweenAttacks = 4.0f;
    public float attackDuration = 1.0f;
    public float attackTargetSpeed = 3.0f;
    public float attackTargetDamping = .5f;
    public float rotateTime = 7;
    
    [Header("Transforms")]
    public Transform centerOfArea;
    public Transform bodyHinge;
    public Transform behindPlayerTarget;
    public Transform head;
    public Transform headCenter;

    private void Start() {
        behindPlayerTarget.SetParent(null, true);
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        if (InCombat) {
            if (Time.time > nextMove) {
                StartCoroutine(MoveToLocation());
            }
            if (Time.time > nextAttack) {
                StartCoroutine(Attack());
            }

            if (attacking) {
                MoveAttackThing(false);
            }

            if (knockedBack) {
                LeanTowards(knockbackTarget);
            }
            else {
                // lean towards target
                LeanTowards(transformTarget.position);
            }

            if (ShouldMove && (transform.position - targetPosition).magnitude > .25f) {
                LookAt(targetPosition);
            }
        }
    }

    private void MoveAttackThing(bool force) {
        Vector3 towardsPlayer = Movement.GetPlayerPos() - headCenter.position;
        towardsPlayer.y = 0;
        towardsPlayer.Normalize();
        Vector3 finalPos = player.transform.position + towardsPlayer * 2.5f;

        ClampTargetPos(ref finalPos);

        if (force) {
            behindPlayerTarget.position = finalPos;
        }
        else {
            behindPlayerTarget.position = Vector3.SmoothDamp(behindPlayerTarget.position, finalPos, ref attackTargetVel,
                attackTargetDamping, attackTargetSpeed);
        }
    }

    private void ClampTargetPos(ref Vector3 finalPos) {
        float size = 7.0f;
        if (finalPos.x > centerOfArea.position.x + size) {
            finalPos.x = centerOfArea.position.x + size;
        }
        if (finalPos.z > centerOfArea.position.z + size) {
            finalPos.z = centerOfArea.position.z + size;
        }
        
        if (finalPos.x < centerOfArea.position.x - size) {
            finalPos.x = centerOfArea.position.x - size;
        }
        if (finalPos.z < centerOfArea.position.z - size) {
            finalPos.z = centerOfArea.position.z - size;
        }
    }
    
    private void LeanTowards(Vector3 targetPos) {
        Vector3 worldLean = Vector3.up + velTarget * leanFactor;
        worldLean.Normalize();

        // Convert the world-space up vector to local space
        Vector3 localUp = bodyHinge.InverseTransformDirection(Vector3.up);
        
        // Convert the target world direction to local space
        Vector3 localTargetDir = bodyHinge.InverseTransformDirection(worldLean);
        
        // Calculate the rotation from localUp to localTargetDir
        Quaternion localRotation = Quaternion.FromToRotation(localUp, localTargetDir);

        bodyHinge.localRotation = localRotation;
    }

    protected override void HandleCombatStart() {
        base.HandleCombatStart();
        nextMove = Time.time + firstMoveTime;
        nextAttack = Time.time + firstAttackTime;
        ShouldMove = false;
    }
    
    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateTime);
    }

    private IEnumerator Attack() {
        nextAttack = Time.time + timeBetweenAttacks;
        attacking = true;
        
        behindPlayerTarget.gameObject.SetActive(true);
        MoveAttackThing(true);
        yield return new WaitForSeconds(attackDuration);
        behindPlayerTarget.gameObject.SetActive(true);
    }

    private IEnumerator MoveToLocation() {
        nextMove = Time.time + timeBetweenMoves;

        ShouldMove = true;
        Vector3 targetPos = GetRandomSpot();
        while ((transform.position - targetPos).magnitude < 4) {
            targetPos = GetRandomSpot();
        }

        knockbackFactorCode = 0;
        targetPosition = targetPos;
        while ((transform.position - targetPosition).magnitude > .25f) {
            yield return null;
        }
        
        Debug.Log("Stopped moving");

        targetPosition = transform.position;
        ShouldMove = false;

        knockbackFactorCode = 1;
    }

    private Vector3 GetRandomSpot() {
        return centerOfArea.position + new Vector3(Random.Range(-5.0f, 6.0f), 0, Random.Range(-5.0f, 6.0f));
    }
}
