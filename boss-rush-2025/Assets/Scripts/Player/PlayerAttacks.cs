using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour {
    public Transform flailRotation;
    public Transform flailForwardRotation;
    public Movement Movement;

    public FlailMovement flail;

    public TrailRenderer trail;

    public InputActionReference attackAction;

    public Material trailMat;

    public static readonly string TWEEN_ID = "PLAYER_ATTACKS";

    [Header("stats")] 
    [Range(0, .5f)] public float swingHitTime = .15f;
    [Range(0, .5f)]
    public float swingTime = .15f;
    [Range(0, 3f)]
    public float returnDelay = .2f;
    [Range(0, .5f)]
    public float returnTime = .2f;

    [Range(0, 90)] 
    public float swingAngle = 70;
    
    [Header("third attack")]
    public float thirdAttackSwingTime = .3f;
    public float thirdAttackRestTime = .25f;
    
    [Header("colors")]
    [ColorUsage(true, true)]
    public Color firstStrikeColor = Color.white;
    [ColorUsage(true, true)]
    public Color secondStrikeColor = Color.white;
    [ColorUsage(true, true)]
    public Color thirdStrikeColor = Color.white;

    public DamageInstance damageInstancePrefab;

    private static PlayerAttacks instance; 
    
    private enum StrikePhase {
        none,
        first,
        second,
        third
    }

    private StrikePhase nextPhase = StrikePhase.first;
    private StrikePhase currentAttack = StrikePhase.none;
    public bool midSwing = false;

    private void Awake() {
        trail.emitting = false;
        instance = this;
    }
    
    private void OnEnable() {
        attackAction.action.started += Attack;
    }

    private void OnDisable() {
        attackAction.action.started -= Attack;
    }

    public static void BriefPause(float pauseDuration) {
        instance.StartCoroutine(instance.briefPauseC(pauseDuration));
    }

    private IEnumerator briefPauseC(float pauseDuration) {
        Color c;
        if (currentAttack == StrikePhase.first) {
            c = firstStrikeColor;
        }
        else if (currentAttack == StrikePhase.second) {
            c = secondStrikeColor;
        }
        else{
            c = thirdStrikeColor;
        }
        SplatManager.Instance.Get().Setup(trail.transform.position, c);

        if (pauseDuration == 0) yield break;
        
        DOTween.Pause(TWEEN_ID);
        yield return new WaitForSeconds(pauseDuration);
        DOTween.Play(TWEEN_ID);
    }
    
    private void Attack(InputAction.CallbackContext obj) {
        if (midSwing) return;
        if (nextPhase == StrikePhase.first) {
            FirstAttack();
        }
        else if (nextPhase == StrikePhase.second) {
            SecondAttack();
        }
        else if (nextPhase == StrikePhase.third) {
            ThirdAttack();
        }
    }

    private void FirstAttack() {
        Movement.Attacking = true;
        currentAttack = StrikePhase.first;
        nextPhase = StrikePhase.second;
        midSwing = true;
        trailMat.color = firstStrikeColor;
        Movement.ForceToFaceInputOrMouse();

        flailForwardRotation.DOKill();
        flailRotation.DOKill();

        GameObject damageInstance = null;
        
        flailForwardRotation.DOLocalRotate(new Vector3(swingAngle, 0, 0), swingTime * .2f).SetId(TWEEN_ID).From(Vector3.zero).OnComplete(() => {
            
            if(currentAttack == StrikePhase.first)
                flailForwardRotation.DOLocalRotate(Vector3.zero, swingTime * .2f).SetId(TWEEN_ID).SetDelay(swingTime);
        });
        flailRotation.DOLocalRotate(new Vector3(0, -179, 0), swingTime).SetId(TWEEN_ID).From(Vector3.zero).SetDelay(swingTime * .2f).SetEase(Ease.InQuad).OnStart(() => {
                trail.emitting = true;
                var newDamageInstance = Instantiate(damageInstancePrefab);
                newDamageInstance.Setup(10, trail.transform, Movement.transform, 100);
                damageInstance = newDamageInstance.gameObject;
                flail.flailSpeed = 0;
        }).OnComplete((() => {
            if (damageInstance) {
                Destroy(damageInstance);
            }
            
            midSwing = false;
            if (currentAttack == StrikePhase.first) {
                Movement.Attacking = false;
                trail.emitting = false;
                flail.flailSpeed = 700;
                flailRotation.DOLocalRotate(new Vector3(0, 0, 0), returnTime).SetDelay(returnDelay).OnComplete(() => {
                    if (currentAttack == StrikePhase.first) {
                        currentAttack = StrikePhase.none;
                        nextPhase = StrikePhase.first;
                    }
                });
            }
        }));
    }

    private void SecondAttack() {
        Movement.Attacking = true;
        currentAttack = StrikePhase.second;
        nextPhase = StrikePhase.third;
        midSwing = true;
        
        GameObject damageInstance = null;

        trailMat.color = secondStrikeColor;
        Movement.ForceToFaceInputOrMouse();
        
        flailForwardRotation.DOKill();
        flailRotation.DOKill();
        
        flailForwardRotation.DOLocalRotate(new Vector3(swingAngle, 0, 0), swingTime * .2f).SetId(TWEEN_ID).OnComplete(() => {
            if(currentAttack == StrikePhase.second)
                flailForwardRotation.DOLocalRotate(Vector3.zero, swingTime * .2f).SetId(TWEEN_ID).SetDelay(swingTime);
        });
        flailRotation.DOLocalRotate(new Vector3(0, 0, 0), swingTime).SetId(TWEEN_ID).SetDelay(swingTime * .2f).SetEase(Ease.InQuad).OnStart(
            () => {
                trail.emitting = true;
                var newDamageInstance = Instantiate(damageInstancePrefab);
                newDamageInstance.Setup(15, trail.transform, Movement.transform, 100);
                damageInstance = newDamageInstance.gameObject;
                flail.flailSpeed = 0;
                
            }).OnComplete((() => {
            midSwing = false;
            if (currentAttack == StrikePhase.second) {
                Movement.Attacking = false;
                trail.emitting = false;
                flail.flailSpeed = 700;
                
                if (damageInstance) {
                    Destroy(damageInstance);
                }
                
                nextPhase = StrikePhase.third;
                // Do a delay, if they don't attack again, then go back to phase 1
                flailRotation.DOLocalRotate(new Vector3(0, 0, 0), returnDelay).OnComplete(() => {
                    if (currentAttack == StrikePhase.second) {
                        nextPhase = StrikePhase.first;
                        currentAttack = StrikePhase.none;
                    }
                });
            }
        }));
    }
    
    private void ThirdAttack() {
        Movement.Attacking = true;
        currentAttack = StrikePhase.third;
        nextPhase = StrikePhase.second;
        midSwing = true;

        trailMat.color = thirdStrikeColor;
        Movement.ForceToFaceInputOrMouse();
        
        flailForwardRotation.DOKill();
        flailRotation.DOKill();

        GameObject damageInstance = null;
        
        flailForwardRotation.DOLocalRotate(new Vector3(swingAngle, 0, 0), swingTime * .2f).OnComplete(() => {
            if(currentAttack == StrikePhase.third)
                flailForwardRotation.DOLocalRotate(Vector3.zero, swingTime * .2f).SetId(TWEEN_ID).SetDelay(thirdAttackSwingTime);
        });
        flailRotation.DOLocalRotate(new Vector3(0, -170, 0), thirdAttackSwingTime * .33f).SetId(TWEEN_ID).SetDelay(thirdAttackSwingTime * .1f).SetEase(Ease.InQuad).OnStart(
            () => {
                trail.emitting = true;
                var newDamageInstance = Instantiate(damageInstancePrefab);
                newDamageInstance.Setup(20, trail.transform, Movement.transform, 100);
                newDamageInstance.AffectSize(1.5f);
                damageInstance = newDamageInstance.gameObject;
                flail.flailSpeed = 0;
            }).OnComplete((() => {
            flailRotation.DOLocalRotate(new Vector3(0, -340, 0), thirdAttackSwingTime * .33f).SetId(TWEEN_ID).SetEase(Ease.Linear).OnComplete(() => {
                flailRotation.DOLocalRotate(new Vector3(0, -150, 0), thirdAttackSwingTime * .33f).SetId(TWEEN_ID).SetEase(Ease.OutQuad)
                    .OnComplete(() => {
                        Movement.Attacking = false;
                        trail.emitting = false;
                        flail.flailSpeed = 700;
                        
                        if (damageInstance) {
                            Destroy(damageInstance);
                        }

                        flailRotation.DOLocalRotate(new Vector3(0, 0, 0), thirdAttackRestTime).OnComplete(() => {
                            currentAttack = StrikePhase.none;
                            nextPhase = StrikePhase.first;
                            midSwing = false;
                        });
                    });
            });
        }));
    }
}
