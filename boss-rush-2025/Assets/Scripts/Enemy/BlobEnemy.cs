using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

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

    [Header("blobs")] 
    public BlobSmallEnemy blobPrefab;
    public float blobSpawnRate;
    public int maxBlobcount;
    public float searchForBlobsInTheWay = .25f;

    private float nextSearchForBlobsInTheWay;

    public Transform centerTarget;
    
    public GameObject bossCam;

    private BlobAttack nextAttack;
    private float nextAttackTime = 3;
    private float nextBlobSpawn = 3;

    private bool attacking = false;

    public List<BlobSmallEnemy> spawnedBlobs;

    protected override void Awake() {
        base.Awake();
        blobAttackInstance.gameObject.SetActive(false);
        BlobSmallEnemy.blobAttacks.Clear();
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

            if (Time.time > nextBlobSpawn) {
                for (var index = 0; index < spawnedBlobs.Count; index++) {
                    var blob = spawnedBlobs[index];
                    if (blob == null) {
                        spawnedBlobs.RemoveAt(index);
                        index--;
                    }
                }

                SpawnBlob();
                nextBlobSpawn = Time.time + ((CurrentHealth / maxHealth) > .5f ? blobSpawnRate : .66f * blobSpawnRate);
            }

            if (Time.time > nextSearchForBlobsInTheWay) {
                SearchForBlobs();
                nextSearchForBlobsInTheWay = Time.time + searchForBlobsInTheWay;
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
            if (towardsPlayer.magnitude > distanceAway) {
                targetPosition = transform.position + (towardsPlayer.normalized * speed);
            }
            else {
                targetPosition = transform.position;
            }
        }
    }

    private void SearchForBlobs() {
        Vector3 towardsPlayer = Movement.GetPlayerPos() - transform.position;

        float disToPlayer = towardsPlayer.magnitude;

        for (var index = 0; index < spawnedBlobs.Count; index++) {
            var blob = spawnedBlobs[index];
            if (blob == null) {
                spawnedBlobs.RemoveAt(index);
                index--;
                continue;
            }

            if (blob.IsJumping) continue;
            Vector3 towardsBlob = blob.transform.position - transform.position;
            if (towardsBlob.magnitude > disToPlayer) continue;
            
            float dot = Vector3.Dot(towardsPlayer.normalized, towardsBlob.normalized);
            Debug.Log(dot);
            if (dot > .5f) {
                blob.Jump();
            }
        }
    }

    protected override void Die() {
        base.Die();
        bossCam.gameObject.SetActive(false);

        foreach (var blob in spawnedBlobs) {
            if (blob != null) {
                blob.Kill();
            }
        }

        GameManager.BlobDefeated = true;
    }

    protected override void HandleCombatStart() {
        base.HandleCombatStart();
        nextAttackTime = Time.time + attackCooldown;
        bossCam.gameObject.SetActive(true);
        
        Invoke(nameof(SpawnBlob), .25f);
        Invoke(nameof(SpawnBlob), .66f);
        Invoke(nameof(SpawnBlob), .8f);

        nextBlobSpawn = Time.time + (blobSpawnRate * 2);
        nextSearchForBlobsInTheWay = Time.time + searchForBlobsInTheWay;
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

        TextPopups.Instance.Get().PopupAbove("!", transform.position + Vector3.one, .5f).SetColor(Color.red).SetSize(3);
        
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

    public void SpawnBlob() {
        if (spawnedBlobs.Count >= maxBlobcount) {
            return;
        }
        
        Vector2 onUnitCircle = Random.insideUnitCircle.normalized;
        Vector3 spawnLocation = new Vector3(onUnitCircle.x, 0, onUnitCircle.y) * 15 + // on the edge
                                centerTarget.position + // Move it to the middle
                                Vector3.down * 2; // Down a little so it's in the lava
        
        var newBlob = Instantiate(blobPrefab, spawnLocation, quaternion.identity);
        spawnedBlobs.Add(newBlob);
        newBlob.centerTarget = centerTarget;
        newBlob.parentBlob = this;
    }
}
