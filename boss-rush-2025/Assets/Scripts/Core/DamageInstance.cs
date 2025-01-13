using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class DamageInstance : MonoBehaviour {
    public LayerMask damageLayerMask;
    public float damage;

    public bool IsFromPlayer;
    public Transform Source;

    public Transform followTarget;
    
    private List<IDamagable> hitObjects = new();

    private bool playerBriefStoppedThisAttack = false;

    public void LateUpdate() {
        if (followTarget) {
            transform.position = followTarget.position;
        }
    }

    public void Reset() {
        hitObjects.Clear();
        followTarget = null;
        playerBriefStoppedThisAttack = false;
    }


    public void Setup(float _damage, Transform _followTarget, Transform source, float destroyAfter) {
        damage = _damage;
        followTarget = _followTarget;
        Source = source;
        
        Destroy(this.gameObject, destroyAfter);
    }
    
    public void OnTriggerEnter(Collider other) {
        if ((damageLayerMask & (1 << other.gameObject.layer)) != 0) {
            if (other.TryGetComponent(out IDamagable damagable)) {
                if (hitObjects.Contains(damagable)) {
                    return;
                }
                hitObjects.Add(damagable);

                if (IsFromPlayer) {
                    if (!playerBriefStoppedThisAttack) {
                        playerBriefStoppedThisAttack = true;
                        PlayerAttacks.BriefPause();
                    }
                }
                
                
                damagable.TakeDamage(damage, Source != null ? Source : transform);
            }
        }
    }
}
