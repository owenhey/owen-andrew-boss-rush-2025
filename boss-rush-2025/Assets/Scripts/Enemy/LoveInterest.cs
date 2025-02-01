using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class LoveInterest : Enemy {
    public Transform ikTarget;

    private float NextAttackTime = 10000;

    public Transform defaultRosePos;
    [Header("Swing 1")]
    public Transform attackStart;
    public Transform attackControlPoint;
    public Transform attackEnd;
    [Header("Swing 2")]
    public Transform attack2Start;
    public Transform attack2Rest;
    public Transform attack2ControlPoint;
    [Header("Swing 3")]
    public Transform swing3extend;
    public Transform bigJumpRotate;
    
    [Header("center thing")]
    public Transform centerAimHigh;

    public float attackDuration = .35f;

    private float swingStart = 0.0f;
    private float swingEnd = 0.0f;

    private int swingPhase = 0;

    public float perlSpeed = 3;
    public float perlAmp = .05f;

    public DamageInstance swing12attack;
    public DamageInstance swing3attack;

    public SpinnerEnemy spinnerPrefab;

    public Transform spinSpawnCenter;

    private float nextSpinSpawn;

    private List<SpinnerEnemy> spawnedSpinners = new();

    private bool wentToMiddle = false;
    private bool inMiddleThing = false;

    public ParticleSystem PS;

    public RockEnemy[] rocks;
    
    
    
    protected override void OnAwake() {
        NextAttackTime = Time.time + 2.0f;
        
        swing12attack.gameObject.SetActive(false);
        swing3attack.gameObject.SetActive(false);

        ShowHealthBar();
        
        nextSpinSpawn = Time.time + 6.0f;
        
        PS.gameObject.SetActive(false);
        
        foreach (var rockEnemy in rocks) {
            rockEnemy.gameObject.SetActive(false);
        }
    }

    protected override void OnUpdate() {
        base.OnUpdate();

        if (inMiddleThing) {
            return;
        }
        
        if (Time.time > NextAttackTime) {
            StartCoroutine(Swing());
        }

        if (swingPhase < 4) {
            LookAt(player.transform.position);
        }

        if (swingPhase < 1) {
            Vector3 towardsPlayer = player.transform.position - transform.position;
            towardsPlayer.y = 0;
            if (towardsPlayer.magnitude > 2) {
                targetPosition = player.transform.position;
            }
            else {
                targetPosition = transform.position;
            }
        }
        else {
            targetPosition = transform.position;
        }

        if (InCombat) {
            if (Time.time > nextSpinSpawn) {
                SpawnSpinner();
            }

            if (!wentToMiddle && CurrentHealth / maxHealth < .5f) {
                StopAllCoroutines();
                StartCoroutine(GoToMiddle());
            }
            
            
            if (swingPhase == 1) {
                if (Time.time > swingEnd) {
                    swingPhase = 2;
                    
                    swing12attack.gameObject.SetActive(false);
                    
                    ikTarget.DOLocalMove(attack2Rest.localPosition, .4f).SetEase(Ease.OutCubic).OnComplete(() => {
                        ikTarget.DOLocalMove(attack2Start.localPosition, .15f).OnComplete(() => {
                            swingStart = Time.time;
                            swingEnd = Time.time + attackDuration;
                            swingPhase = 3;
                            
                            swing12attack.Reset();
                            swing12attack.gameObject.SetActive(true);
                        });
                    });
                }
                else {
                    float t = 1 - ((swingEnd - Time.time) / attackDuration);
                    ikTarget.localPosition = GetLocalSwing1Position(t);
                }
            }

            if (swingPhase == 3) {
                if (Time.time > swingEnd) {
                    swingPhase = 4;
                    swing12attack.gameObject.SetActive(false);
                    
                    ikTarget.DOLocalMove(swing3extend.localPosition, .1f).OnStart(()=> {
                        var v = JumpTowardsPlayerPos();
                        if ((v - transform.position).magnitude > 1.5f) {
                            JumpToPos(v);
                        }
                    }).SetDelay(.4f).OnComplete(() => {
                        swingStart = Time.time;
                        swingEnd = Time.time + attackDuration * 3;
                        swingPhase = 5;

                        ikTarget.DOLocalMove(swing3extend.localPosition + Vector3.up * .25f, .15f).OnComplete(() => {
                            swing3attack.gameObject.SetActive(true);
                            swing3attack.Reset();
                            
                            bigJumpRotate.DOLocalRotate(new Vector3(0, 120, 0), attackDuration * .6f).SetEase(Ease.InQuad).OnComplete(() => {
                                swing3attack.Reset();
                                bigJumpRotate.DOLocalRotate(new Vector3(0, 240, 0), attackDuration * .6f).SetEase(Ease.Linear).OnComplete(() => {
                                    swing3attack.Reset();
                                    bigJumpRotate.DOLocalRotate(new Vector3(0, 360, 0), attackDuration * .6f).SetEase(Ease.OutQuad).OnComplete(() => {
                                        swing3attack.gameObject.SetActive(false);
                                        
                                        ikTarget.DOLocalMove(defaultRosePos.localPosition, .2f);
                                        knockbackFactorCode = 1;
                                        swingPhase = 0;
                                    });
                                });
                            });
                        });
                    });
                }
                else {
                    float t = 1 - ((swingEnd - Time.time) / attackDuration);
                    ikTarget.localPosition = GetLocalSwing2Position(t);
                }
            }
        }
    }
    
    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 9);
    }

    private void JumpToPos(Vector3 pos) {
        ShouldMove = false;
        cc.enabled = false;
        transform.DOJump(pos, .5f, 1, .3f);
        ShouldMove = true;
        cc.enabled = true;

        targetPosition = pos;
        transformTarget.position = pos;
    }

    
    
    
    
    
    
    
    
    
    private IEnumerator GoToMiddle() {
        canBeDamaged = false;
        inMiddleThing = true;
        yield return new WaitForSeconds(.5f);
        ShouldMove = false;
        
        swing3attack.gameObject.SetActive(false);
        swing12attack.gameObject.SetActive(false);
        wentToMiddle = true;
        
        JumpToPos(spinSpawnCenter.position);

        foreach (var spinner in spawnedSpinners) {
            if(spinner != null)
                spinner.Kill();
        }

        spawnedSpinners.Clear();
        
        foreach (var rockEnemy in rocks) {
            rockEnemy.gameObject.SetActive(true);
        }

        ikTarget.DOLocalMove(centerAimHigh.localPosition, .5f).SetDelay(.75f);
        knockbackFactorCode = 0;

        float healAmount = 250;
        float duration = 20;

        yield return new WaitForSeconds(1.25f);
        PS.gameObject.SetActive(true);
        
        for (int i = 0; i < 30; i++) {
            yield return new WaitForSeconds(duration / 30.0f);
            
            TakeDamage(-(healAmount / 30.0f), transform, true);
        }
        
        PS.gameObject.SetActive(false);
        canBeDamaged = true;
        inMiddleThing = false;
        ShouldMove = true;
        
        nextSpinSpawn = Time.time + 10.0f;
        
        foreach (var rockEnemy in rocks) {
            if (rockEnemy != null) {
                Destroy(rockEnemy.gameObject);
            }
        }

        yield return new WaitForSeconds(.25f);
        SpawnSpinner();
        yield return new WaitForSeconds(.25f);
        SpawnSpinner();
        yield return new WaitForSeconds(.25f);
        SpawnSpinner();
        lotsOfSpinners = true;
        
        nextSpinSpawn = Time.time + 6.0f;
    }








    private bool lotsOfSpinners = false;

    private void SpawnSpinner() {
        Debug.Log("Spawning spinner");
        for (var index = 0; index < spawnedSpinners.Count; index++) {
            var spinner = spawnedSpinners[index];
            if (spinner == null) {
                spawnedSpinners.RemoveAt(index);
                index--;
                continue;
            }
        }

        nextSpinSpawn = Time.time +  10.0f * (lotsOfSpinners ? .65f : 1.0f);

        if (spawnedSpinners.Count > (lotsOfSpinners ? 4 : 2)) return;

        var newSpinner = Instantiate(spinnerPrefab);

        Vector3 position = spinSpawnCenter.position +
                           new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
        
        
        while ((player.transform.position - position).magnitude < 3.5f) {
            Debug.Log("Trying to find a new one.");
            position = spinSpawnCenter.position +
                       new Vector3(Random.Range(-5.0f, 5.0f), 0, Random.Range(-5.0f, 5.0f));
        }
        
        Debug.Log("Final dis from player: " + (player.transform.position - position).magnitude);

        newSpinner.transform.position = position;
        spawnedSpinners.Add(newSpinner);
    }

    private Vector3 GetLocalSwing1Position(float t) {
        t = Mathf.Clamp01(t);
        
        t = t * t; // in quad

        float minusT = 1 - t;

        Vector3 pointA = Vector3.Lerp(attackStart.localPosition, attackControlPoint.localPosition, t);
        Vector3 pointB = Vector3.Lerp(attackControlPoint.localPosition, attackEnd.localPosition, t);

        Vector3 localPos = Vector3.Lerp(pointA, pointB, t);
        
        localPos += GetPerlinFunction();

        return localPos;
    }
    
    private Vector3 GetLocalSwing2Position(float t) {
        t = Mathf.Clamp01(t);
        
        t = t * t; // in quad

        float minusT = 1 - t;

        Vector3 pointA = Vector3.Lerp(attack2Start.localPosition, attack2ControlPoint.localPosition, t);
        Vector3 pointB = Vector3.Lerp(attack2ControlPoint.localPosition, attackStart.localPosition, t);

        Vector3 localPos = Vector3.Lerp(pointA, pointB, t);
        
        localPos += GetPerlinFunction();

        return localPos;
    }

    private Vector3 GetPerlinFunction() {
        return perlAmp * new Vector3(
            Mathf.PerlinNoise1D(Time.time * perlSpeed) * 2 - 1, 
            Mathf.PerlinNoise1D(Time.time * perlSpeed + 123123)* 2 - 1,
            Mathf.PerlinNoise1D(Time.time * perlSpeed + 1232)* 2 - 1);
    }

    private IEnumerator Swing() {
        NextAttackTime = Time.time + 5.0f;

        yield return null;

        knockbackFactorCode = 0;

        ikTarget.DOLocalMove(attackStart.localPosition, .25f).OnComplete(() => {
            swingStart = Time.time;
            swingEnd = Time.time + attackDuration;
            swingPhase = 1;
            swing12attack.Reset();
            swing12attack.gameObject.SetActive(true);
        });
    }

    private Vector3 JumpTowardsPlayerPos() {
        Vector3 towardsPlayer = Movement.GetPlayerPos() - transform.position;

        Vector3 towardsMe = -towardsPlayer;
        towardsMe = towardsMe.normalized * 1;

        Vector3 finalPos = player.transform.position + towardsMe;
        finalPos.y = transform.position.y;
        return finalPos;
    }

    /// <summary>
    /// Remaps a value from one range into another using linear interpolation. Clamps the starting values
    /// </summary>
    public static float RemapClamp(float value, float startLow, float startHigh, float endLow, float endHigh) {
        value = Mathf.Clamp(value, startLow, startHigh);
        return endLow + ((endHigh - endLow) / (startHigh - startLow)) * (value - startLow);
    }
}
