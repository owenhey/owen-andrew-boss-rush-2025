using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlobEnemy : Enemy {
    private enum BlobAttack {
        basic,
        hop,
        gooSpin
    }

    [Header("refs")] 
    public Transform blobHead;
    public Transform blobPullBackPos;
    public Transform blobAttackPos;
    public Transform blobRestPos;

    [Header("Stats")] 
    public float turnSpeed = 15;
    public float attackCooldown = 3;
    public float distanceAway = 1.5f;

    [Header("basic")] 
    public DamageInstance blobAttackInstance;
    public float basicAttackTime = .1f;
    public float basicAttackDelay = .5f;

    private BlobAttack nextAttack;
    private float nextAttackTime = 3;

    private bool attacking = false;

    protected override void Awake() {
        base.Awake();
        blobAttackInstance.gameObject.SetActive(false);
    }

    protected override void OnUpdate() {
        if (InCombat) {
            if (!attacking) {
                bool shouldNextAttack = Time.time > nextAttackTime;
                if (shouldNextAttack) {
                    nextAttackTime = Time.time + attackCooldown;
                    StartCoroutine(BasicC());
                }
            }
            else {
                
            }
            Vector3 lookTarget;
            if ((targetPosition - cc.transform.position).magnitude < 1) {
                lookTarget = player.transform.position;
            }
            else {
                lookTarget = targetPosition;
            }
            LookAt(lookTarget);

            Vector3 towardsPlayer = player.transform.position - transform.position;
            towardsPlayer.y = 0;
            Debug.Log("current distance: " + towardsPlayer.magnitude);
            if (towardsPlayer.magnitude > distanceAway) {
                targetPosition = transform.position + (towardsPlayer.normalized * speed);
            }
            else {
                targetPosition = transform.position;
            }
        }
    }

    protected override void HandleCombatStart() {
        base.HandleCombatStart();
        nextAttackTime = Time.time + attackCooldown;
    }

    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    private IEnumerator BasicC() {
        blobAttackInstance.Reset();
        blobAttackInstance.Source = transform;
        
        attacking = true;
        blobHead.DOLocalMove(blobPullBackPos.localPosition, .25f);
        yield return new WaitForSeconds(basicAttackDelay);
        blobHead.DOLocalMove(blobAttackPos.localPosition, basicAttackTime).SetEase(Ease.OutBounce).OnComplete(() => {
            blobAttackInstance.gameObject.SetActive(true);
            blobHead.DOLocalMove(blobRestPos.localPosition, .1f).OnComplete(() => {
                blobAttackInstance.gameObject.SetActive(false);
            });
        });
        yield return new WaitForSeconds(basicAttackTime + .15f);
        attacking = false;
    }
}
