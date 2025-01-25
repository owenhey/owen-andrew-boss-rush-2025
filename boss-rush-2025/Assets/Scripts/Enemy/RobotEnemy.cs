using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RobotEnemy : Enemy {
    private float nextMove = float.MaxValue;
    private float nextAttack = float.MaxValue;
    private bool attacking = false;
    private bool spinAttacking = false;
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
    public float laserDelay = .3f;
    public float spinDelay = .3f;
    public float rotateTime = 7;
    public float spinAttackRotateSpeed = 30;
    public float spinAttackDuration = 3;
    public int SpinLimbCount = 4;
    
    [Header("Transforms")]
    public Transform centerOfArea;
    public Transform bodyHinge;
    public Transform behindPlayerTarget;
    public Transform head;
    public Transform headCenter;
    public Transform smile;
    public DamageInstance laser;
    public Transform laserParent;
    public Transform spinLaserInstance;
    public Transform spinParent;

    [ColorUsage(true, true)] public Color regLaserColor;
    [ColorUsage(true, true)] public Color activeLaserColor;
    public Material laserMat;
    
    private int attackCount = 0;

    private List<(Transform, DamageInstance)> spinAttacks = new();

    private void Start() {
        behindPlayerTarget.SetParent(null, true);
        laser.gameObject.SetActive(false);

        spinParent.gameObject.SetActive(false);
        spinAttacks.Add((spinLaserInstance, spinLaserInstance.GetComponentInChildren<DamageInstance>(true)));
        for (int i = 0; i < SpinLimbCount - 1; i++) {
            float rotation = 360.0f / (SpinLimbCount);
            var newLaser = Instantiate(spinLaserInstance, spinLaserInstance.transform.parent);
            newLaser.transform.localEulerAngles = new Vector3(0, (i + 1) * rotation, 0);
            spinAttacks.Add((newLaser.transform, newLaser.GetComponentInChildren<DamageInstance>(true)));
        }
    }
    
    protected override void OnUpdate() {
        base.OnUpdate();
        if (InCombat) {
            if (Time.time > nextMove) {
                StartCoroutine(MoveToLocation());
            }
            if (Time.time > nextAttack) {
                attackCount++;
                if (attackCount % 4 == 0) {
                    StartCoroutine(CircleAttack());
                }
                else {
                    StartCoroutine(Attack());
                }
            }

            if (spinAttacking) {
                float factor = CurrentHealth / maxHealth > .5f ? 1.0f : 1.5f;
                spinParent.Rotate(new Vector3(0, factor * spinAttackRotateSpeed * Time.deltaTime, 0));
            }

            if (attacking) {
                MoveAttackThing(false);
                head.LookAt(behindPlayerTarget);
                // laserParent.transform.localScale = Vector3.one + Vector3.one * (Mathf.Sin(Time.time * 15) * .15f);
            }
            else {
                head.LookAt(player.transform.position + Vector3.up * 2);
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

        smile.DOLocalRotate(new Vector3(0, 0, 180), .5f);
        ShouldMove = false;
    }
    
    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotateTime);
    }
    
    private IEnumerator CircleAttack() {
        spinAttacking = true;
        Debug.Log("Circle attack");
        nextAttack = Time.time + timeBetweenAttacks + spinAttackDuration;

        laserMat.SetColor("_Color", regLaserColor);
        spinParent.gameObject.SetActive(true);

        foreach (var spinAttack in spinAttacks) {
            spinAttack.Item2.Reset();
            spinAttack.Item2.c.enabled = false;
            spinAttack.Item1.localScale = new Vector3(.25f, .25f, 1.0f);
        }
        laser.Enabled = false;

        yield return new WaitForSeconds(spinDelay);
        foreach (var spinAttack in spinAttacks) {
            spinAttack.Item1.DOScale(Vector3.one, .15f);
            spinAttack.Item2.c.enabled = true;
        }
        laserMat.DOColor(activeLaserColor, "_Color", .15f);
        
        for (int i = 0; i < 10; i++) {
            foreach (var spinAttack in spinAttacks) {
                spinAttack.Item2.SingleSwipe();
            }
            yield return new WaitForSeconds(spinAttackDuration * .1f);
        }
        foreach (var spinAttack in spinAttacks) {
            spinAttack.Item2.SingleSwipe();
        }
        
        spinAttacking = false;
        spinParent.gameObject.SetActive(false);
    }

    private IEnumerator Attack() {
        nextAttack = Time.time + timeBetweenAttacks;
        attacking = true;

        laserMat.SetColor("_Color", regLaserColor);
        laser.Enabled = false;
        laser.Reset();
        laser.c.enabled = false;
        MoveAttackThing(true);
        laserParent.localScale = new Vector3(.25f, .25f, 1.0f);
        laser.gameObject.SetActive(true);
        head.LookAt(behindPlayerTarget.position);

        
        float d = .25f;
        yield return new WaitForSeconds(laserDelay - d);
        TextPopups.Instance.Get().PopupAbove("!", transform, .25f).SetColor(Color.red).SetSize(4.0f);
        yield return new WaitForSeconds(d);
        
        laserMat.DOColor(activeLaserColor, "_Color", .1f);
        laser.SingleSwipe();
        laser.c.enabled = true;
        laser.Enabled = true;
        behindPlayerTarget.gameObject.SetActive(true);
        laserParent.DOScale(new Vector3(1.0f, 1.0f, 1.0f), .1f);
        yield return new WaitForSeconds(attackDuration);
        laser.SingleSwipe();
        laser.Enabled = false;
        laser.gameObject.SetActive(false);
        attacking = false;
        behindPlayerTarget.gameObject.SetActive(true);
    }

    private IEnumerator MoveToLocation() {
        nextMove = Time.time + timeBetweenMoves;
        if (spinAttacking) yield break;

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
