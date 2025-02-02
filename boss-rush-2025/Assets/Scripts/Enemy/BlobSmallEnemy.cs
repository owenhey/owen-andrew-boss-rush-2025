using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BlobSmallEnemy : Enemy {
    [Header("refs")] 
    public Transform blobHead;

    [Header("basic")] 
    public Transform blobAttackIndicator;
    public DamageInstance blobAttackInstance;
    public float jumpAttackTime = .1f;
    public float jumpAttackDelay = .5f;
    public float timebetweenjumps = 3;
    private float nextJump = 3;
    public float jumpHeight = 5;
    public float jumpTime = .75f;
    public float chanceToJumpNearPlayer = .5f;

    [Header("transforms")] 
    public Transform headDownPos;
    public Transform headRegularPos;
    public Transform headReallyDownPos;

    public Transform centerTarget;
    public float radius;
    public BlobEnemy parentBlob;

    public DamageInstance damageInstance;

    public static List<Transform> blobAttacks = new();

    public bool IsJumping;

    protected override void Awake() {
        base.Awake();
        blobAttackInstance.gameObject.SetActive(false);
        InCombat = true;
        nextJump = Time.time;
        ShouldMove = false;
        
        blobAttackIndicator.gameObject.SetActive(false);
        blobAttackIndicator.transform.SetParent(null, true);
        damageInstance.gameObject.SetActive(false);
        
        blobAttacks.Add(blobAttackIndicator);
        
        transform.Rotate(0, Random.Range(0, 359), 0);
    }

    protected override void OnUpdate() {
        if (Time.time > nextJump) {
            Jump();
        }
    }

    protected override void Die() {
        base.Die();
        blobHead.DOKill();
        transform.DOKill();
        Destroy(blobAttackIndicator.gameObject);

        blobAttacks.Remove(blobAttackIndicator);
    }

    public void Jump() {
        StartCoroutine(JumpC());
    }

    private IEnumerator JumpC() {
        IsJumping = true;
        nextJump = Time.time + timebetweenjumps * Random.Range(1.0f, 1.2f);
        
        blobAttackInstance.Reset();
        blobAttackInstance.Source = transform;

        blobHead.DOLocalMove(headDownPos.localPosition, .35f);
        yield return new WaitForSeconds(.4f);
        
        
        blobHead.DOLocalMove(headRegularPos.localPosition, .25f);

        knockedBack = false;
        knockbackFactorCode = 0;

        Vector3 randomSpot = GetJumpPosition();
        
        blobAttackIndicator.transform.position = randomSpot;
        blobAttackIndicator.gameObject.SetActive(true);

        transform.DOJump(randomSpot, jumpHeight, 1, jumpTime).SetEase(Ease.Linear);
        
        yield return new WaitForSeconds(jumpTime);
        
        blobAttackIndicator.gameObject.SetActive(false);
        knockbackFactorCode = 1;
        blobHead.DOLocalMove(headReallyDownPos.localPosition, .25f).SetEase(Ease.OutQuad).OnComplete(() => {
            blobHead.DOLocalMove(headRegularPos.localPosition, .2f).SetEase(Ease.InOutQuad);
        });
        damageInstance.SingleSwipe();
        Sound.I.PlayBlobSquish();
        IsJumping = false;
    }

    private Vector3 GetJumpPosition() {
        Vector3 playerPos = Movement.GetPlayerPos();
        
        bool oneNearPlayer = blobAttacks.Exists(x => (x.position - playerPos).sqrMagnitude < 2);

        if (!oneNearPlayer && Random.Range(0.0f, 1.0f) < chanceToJumpNearPlayer) {
            return playerPos;
        }
        else {
            Vector3 tryValue = GetInRange(centerTarget.position, radius);
            int i = 0;
            while (true) {
                bool oneIsTooClose = blobAttacks.Exists(x => (x.position - tryValue).sqrMagnitude < 4);
                if (!oneIsTooClose) {
                    return tryValue;
                }
                tryValue = GetInRange(centerTarget.position, radius);
                i++;
                if (i > 10) {
                    return tryValue;
                }
            }
        }
    }

    private Vector3 GetInRange(Vector3 middle, float _radius) {
        var insideUnitCircle = Random.insideUnitCircle;
        Vector3 circle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        Vector3 randomSpot = (circle * _radius) + middle;
        return randomSpot;
    }
}
