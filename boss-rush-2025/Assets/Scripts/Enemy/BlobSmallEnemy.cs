using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlobSmallEnemy : Enemy {
    [Header("refs")] 
    public Transform blobHead;

    [Header("basic")] 
    public DamageInstance blobAttackInstance;
    public float jumpAttackTime = .1f;
    public float jumpAttackDelay = .5f;
    public float timebetweenjumps = 3;
    private float nextJump = 3;
    public float jumpHeight = 5;
    public float jumpTime = .75f;

    [Header("transforms")] 
    public Transform headDownPos;
    public Transform headRegularPos;
    public Transform headReallyDownPos;

    public Transform centerTarget;
    public float radius;
    public BlobEnemy parentBlob;

    public DamageInstance damageInstance;

    protected override void Awake() {
        base.Awake();
        blobAttackInstance.gameObject.SetActive(false);
        InCombat = true;
        nextJump = Time.time;
        ShouldMove = false;
        damageInstance.gameObject.SetActive(false);
        damageInstance.transform.SetParent(null, true);
        
        transform.Rotate(0, Random.Range(0, 359), 0);
    }

    protected override void OnUpdate() {
        if (Time.time > nextJump) {
            nextJump = Time.time + timebetweenjumps * Random.Range(1.0f, 1.2f);
            StartCoroutine(JumpC());
        }
    }

    protected override void Die() {
        base.Die();
        blobHead.DOKill();
        transform.DOKill();
        Destroy(damageInstance.gameObject);
    }

    private IEnumerator JumpC() {
        blobAttackInstance.Reset();
        blobAttackInstance.Source = transform;

        blobHead.DOLocalMove(headDownPos.localPosition, .35f);
        yield return new WaitForSeconds(.4f);
        
        blobHead.DOLocalMove(headRegularPos.localPosition, .25f);

        knockedBack = false;
        knockbackFactorCode = 0;
        
        var insideUnitCircle = Random.insideUnitCircle;
        Vector3 circle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        Vector3 randomSpot = (circle * radius) + centerTarget.transform.position;
        damageInstance.gameObject.SetActive(true);
        damageInstance.transform.position = randomSpot;
        transform.DOJump(randomSpot, jumpHeight, 1, jumpTime).SetEase(Ease.Linear);
        yield return new WaitForSeconds(jumpTime * .9f);
        damageInstance.gameObject.SetActive(false);
        blobHead.DOLocalMove(headReallyDownPos.localPosition, .25f).SetEase(Ease.OutQuad).OnComplete(() => {
            blobHead.DOLocalMove(headRegularPos.localPosition, .2f).SetEase(Ease.InOutQuad);
        });
        
        knockbackFactorCode = 1;
    }
}
