using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Spider : Enemy {
    public List<SpiderLeg> leftLegs;
    public List<SpiderLeg> rightLegs;

    [Range(0, 1)]
    public float switchLegTime = .25f;
    [Range(0, 1)]
    public float legMoveDistance = .25f;
    [Range(0, 1)]
    public float legMoveTime = .15f;

    [Header("transforms")]
    public Transform Body;
    public Transform BodyParent;
    public Transform BodyAttack;
    
    public Vector3 velocity;
    private Vector3 previousPosition;

    private bool seekingPlayer;

    public float minSpiderDistance = 2.0f;
    public Transform areaCenter;
    public DamageInstance DamageInstance;
    public DamageInstance SingleDamageInstance;

    [Header("3 attacks")]
    public float spiderFollowTime = .5f;
    public float spiderSitTime = 1.0f;
    private bool autoMove = false;
    
    
    private float nextAttackTime;
    private float startCombatTime;

    private bool previousWalkingPhase = false;

    public GameObject exitBlocker;

    private int numSmallSpiders = 0;

    public LittleSpider[] smallSpiders;

    private int randomRowStart;
    

    private void Start() {
        for (int i = 0; i < leftLegs.Count; i++) {
            leftLegs[i].spider = rightLegs[i].spider = this;
        }
        randomRowStart = Random.Range(1, 5);
    }

    protected override void OnUpdate(){
        base.OnUpdate();
        if (InCombat) {
            velocity = (transform.position - previousPosition) / Time.deltaTime;
            previousPosition = transform.position;

            LeanTowards(targetPosition);

            if (Time.time > nextAttackTime) {
                StartCoroutine(Spider3AttackCoroutine());
            }
            
            if (autoMove) {
                bool inWalkingPhase = (int)((Time.time - startCombatTime) / 3f) % 2 == 0;
                if (!inWalkingPhase && previousWalkingPhase) {
                    StartCoroutine(SingleSpiderAttack());
                }
                previousWalkingPhase = inWalkingPhase;
                
                
                Vector3 towardsPlayer = player.transform.position - transform.position;
                towardsPlayer.y = 0;
                if (towardsPlayer.magnitude > minSpiderDistance && inWalkingPhase) {
                    targetPosition = player.transform.position;
                }
                else {
                    targetPosition = transform.position;
                }
            }
        }
        
        int count = (int)(Time.time / switchLegTime);
        for (int i = 0; i < leftLegs.Count; i++) {
            bool even = (count + i) % 2 == 0;
            leftLegs[i].CanMove = even;
            rightLegs[i].CanMove = !even;
            
            leftLegs[i].moveThreshold = legMoveDistance;
            rightLegs[i].moveThreshold = legMoveDistance;
            
            leftLegs[i].legMoveTime = legMoveTime;
            rightLegs[i].legMoveTime = legMoveTime;
        }

        float healthPercent = CurrentHealth / maxHealth;
        if (healthPercent < .75f && numSmallSpiders == 0) {
            smallSpiders[0].gameObject.SetActive(true);
            smallSpiders[0].row = randomRowStart % 6;
            smallSpiders[0].transform.SetParent(null, true);
            smallSpiders[0].speed *= Random.Range(1.0f, 1.5f);
            
            randomRowStart += Random.Range(1, 3);
            numSmallSpiders++;
        }
        else if (healthPercent < .55f && numSmallSpiders == 1) {
            smallSpiders[1].gameObject.SetActive(true);
            smallSpiders[1].row = randomRowStart % 6;
            smallSpiders[1].transform.SetParent(null, true);
            smallSpiders[1].speed *= Random.Range(1.0f, 1.5f);
            smallSpiders[1].direction = -1;
            
            randomRowStart += Random.Range(1, 3);
            numSmallSpiders++;
        }
        else if (healthPercent < .3f && numSmallSpiders == 2) {
            smallSpiders[2].gameObject.SetActive(true);
            smallSpiders[2].transform.SetParent(null, true);
            
            smallSpiders[2].row = randomRowStart % 6;
            smallSpiders[2].speed *= Random.Range(1.0f, 1.5f);
            numSmallSpiders++;
        }
        
        LookAt(player.transform.position);
    }

    protected override void HandleCombatStart() {
        base.HandleCombatStart();
        nextAttackTime = Time.time + 6.0f;
        startCombatTime = Time.time;
        autoMove = true;
        exitBlocker.SetActive(true);
    }

    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6);
    }
    
    private void LeanTowards(Vector3 targetPos) {
        Vector3 towards = targetPos - transform.position;
        towards = Vector3.ClampMagnitude(towards, 1);
        towards.y = 0;
        float magnitude = towards.magnitude;

        Vector3 worldLean = Vector3.up + (towards * (magnitude * .08f));
        worldLean.Normalize();

        // Convert the world-space up vector to local space
        Vector3 localUp = Body.InverseTransformDirection(Vector3.up);
        
        // Convert the target world direction to local space
        Vector3 localTargetDir = Body.InverseTransformDirection(worldLean);
        
        // Calculate the rotation from localUp to localTargetDir
        Quaternion localRotation = Quaternion.FromToRotation(localUp, localTargetDir);

        Body.localRotation = localRotation;
    }
    
    private IEnumerator SingleSpiderAttack() {
        TextPopups.Instance.Get().PopupAbove("!", transform, .25f).SetColor(Color.red).SetSize(4.0f);
        yield return new WaitForSeconds(.25f);
        BodyParent.DOLocalMove(BodyAttack.localPosition, .1f).SetEase(Ease.InQuad).OnComplete(() => {
            SingleDamageInstance.SingleSwipe();
            BodyParent.DOLocalMove(Vector3.zero, .1f);
        });
        yield return new WaitForSeconds(.1f);
        // Attack
    }

    protected override void Die() {
        base.Die();
        exitBlocker.SetActive(false);

        GameManager.SpiderDefeated = true;

        foreach (var littlespider in smallSpiders) {
            Destroy(littlespider.gameObject);
        }
    }

    private IEnumerator Spider3AttackCoroutine() {
        nextAttackTime = Time.time + (Random.Range(0.0f, 1.0f) < .5f ? 13.0f : 19.0f);
        knockbackFactorCode = 0.0f;
        autoMove = false;
        
        // Run away from the player for a moment, then run towards really quickly
        Vector3 location = GetRandomSpotInCircle();
        while ((player.transform.position - location).magnitude < 10) {
            location = GetRandomSpotInCircle();
        }
        
        targetPosition = location;
        speedFactor = 4f;
        yield return new WaitForSeconds(1.0f);
        
        speedFactor = 10.0f;
        Vector3 playerPos = Movement.GetPlayerPos();
        Vector3 towardsSpider = (transform.position - playerPos).normalized;
        targetPosition = towardsSpider * (minSpiderDistance * .5f) + playerPos;
        
        yield return new WaitForSeconds(.15f);
        
        DamageInstance.SingleSwipe();
        
        BodyParent.DOLocalMove(BodyAttack.localPosition, .1f).SetEase(Ease.InQuad).OnComplete(() => {
            BodyParent.DOLocalMove(Vector3.zero, .1f);
        });
        
        yield return new WaitForSeconds(.8f);
        
        playerPos = Movement.GetPlayerPos();
        towardsSpider = (transform.position - playerPos).normalized;
        targetPosition = towardsSpider * (minSpiderDistance * .5f) + playerPos;
        
        yield return new WaitForSeconds(.15f);
        
        DamageInstance.SingleSwipe();
        
        BodyParent.DOLocalMove(BodyAttack.localPosition, .1f).SetEase(Ease.InQuad).OnComplete(() => {
            BodyParent.DOLocalMove(Vector3.zero, .1f);
        });
        
        yield return new WaitForSeconds(.5f);
        
        playerPos = Movement.GetPlayerPos();
        towardsSpider = (transform.position - playerPos).normalized;
        targetPosition = towardsSpider * (minSpiderDistance * .5f) + playerPos;
        
        yield return new WaitForSeconds(.15f);
        
        DamageInstance.SingleSwipe();
        
        BodyParent.DOLocalMove(BodyAttack.localPosition, .1f).SetEase(Ease.InQuad).OnComplete(() => {
            BodyParent.DOLocalMove(Vector3.zero, .1f);
        });
        
        speedFactor = 1.0f;
        knockbackFactorCode = 1.0f;
        autoMove = true;
        previousWalkingPhase = false;
        startCombatTime = Time.time;
    }

    public override void HandlePlayerEnterBossZone() {
        base.HandlePlayerEnterBossZone();
        
    }

    private Vector3 GetRandomSpotInCircle() {
        var insideUnitCircle = Random.insideUnitCircle;
        Vector3 circle = new Vector3(insideUnitCircle.x, 0, insideUnitCircle.y);
        Vector3 randomSpot = (circle * 10) + areaCenter.position;
        return randomSpot;
    }
}
