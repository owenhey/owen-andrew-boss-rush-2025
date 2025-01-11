using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttacks : MonoBehaviour {
    public Transform flailRotation;
    public Transform flailForwardRotation;
    public Movement Movement;

    public TrailRenderer trail;

    public InputActionReference attackAction;

    private bool canAttack = true;

    [Header("STats")] 
    [Range(0, .5f)]
    public float swingTime = .15f;
    [Range(0, .5f)]
    public float returnTime = .08f;

    private void Awake() {
        trail.emitting = false;
    }
    
    private void OnEnable() {
        attackAction.action.started += Attack;
    }

    private void OnDisable() {
        attackAction.action.started -= Attack;
    }
    
    private void Attack(InputAction.CallbackContext obj) {
        if (!canAttack) return;
        canAttack = false;
        Movement.Attacking = true;
        
        flailForwardRotation.DOLocalRotate(new Vector3(45, 0, 0), .05f).From(Vector3.zero).OnComplete(() => {
            flailForwardRotation.DOLocalRotate(Vector3.zero, .05f).SetDelay(swingTime).OnComplete(() => {
            });
        });
        flailRotation.DOLocalRotate(new Vector3(0, -180, 0), swingTime).From(Vector3.zero).SetDelay(.05f).SetEase(Ease.InQuad).OnStart(
            () => {
                trail.emitting = true;
            }).OnComplete((() => {
            Movement.Attacking = false;
            trail.emitting = false;
            flailRotation.DOLocalRotate(new Vector3(0, 0, 0), returnTime).SetDelay(returnTime).OnComplete(() => {
                canAttack = true;
            });
        }));
    }
}
