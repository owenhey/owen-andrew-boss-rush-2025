using System.Collections;
using DG.Tweening;
using UnityEngine;

public class BlobSmallEnemy : Enemy {
    [Header("refs")] 
    public Transform blobHead;
    public Transform blobDownPos;

    [Header("basic")] 
    public DamageInstance blobAttackInstance;
    public float jumpAttackTime = .1f;
    public float jumpAttackDelay = .5f;
    private float nextJump = 3;

    public Transform centerTarget;
    public float radius;

    protected override void Awake() {
        base.Awake();
        blobAttackInstance.gameObject.SetActive(false);
    }

    protected override void OnUpdate() {
        if (Time.time > nextJump) {
            nextJump = Time.time + 5;
            StartCoroutine(JumpC());
        }
    }

    private IEnumerator JumpC() {
        blobAttackInstance.Reset();
        blobAttackInstance.Source = transform;

        var insideUnitCircle = Random.insideUnitCircle;
        Vector3 circle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        Vector3 randomSpot = (circle * radius) + centerTarget.transform.position;
        transform.DOJump(randomSpot, 10, 1, .5f);
        yield return null;
    }
}
