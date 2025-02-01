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

    private static Collider[] hitColliders = new Collider[16];

    public Transform capsulePoint1;
    public Transform capsulePoint2;

    public Collider c;

    public bool bypassInvulnerable;

    public Action OnHit;
    public bool Enabled = true;

    private void Awake() {
        c = GetComponent<Collider>();
    }

    public void LateUpdate() {
        if (followTarget) {
            transform.position = followTarget.position;
        }
    }

    public void Reset() {
        hitObjects.Clear();
        followTarget = null;
    }


    public void Setup(float _damage, Transform _followTarget, Transform source, float destroyAfter) {
        damage = _damage;
        followTarget = _followTarget;
        Source = source;
        
        Destroy(this.gameObject, destroyAfter);
    }

    public void AffectSize(float factor) {
        transform.localScale *= factor;
    }

    private float swingDuration = .12f;
    
    public void OnTriggerEnter(Collider other) {
        if (!Enabled) return;
        if ((damageLayerMask & (1 << other.gameObject.layer)) != 0) {
            if (other.TryGetComponent(out IDamagable damagable)) {
                if (hitObjects.Contains(damagable)) {
                    return;
                }
                hitObjects.Add(damagable);

                if (IsFromPlayer) {
                    PlayerAttacks.BriefPause(swingDuration);
                    Sound.I.PlayHit();
                    swingDuration = Mathf.Clamp(swingDuration - .02f, 0, 1.0f);
                }
                
                damagable.TakeDamage(damage, Source != null ? Source : transform, bypassInvulnerable);
                OnHit?.Invoke();
            }
        }
    }

    public void SingleSwipe(bool reset = true) {
        Collider col = GetComponent<Collider>();
        int numHit = 0;
        if (col is SphereCollider sphereCollider) {
            numHit = Physics.OverlapSphereNonAlloc(transform.position, sphereCollider.radius, hitColliders);
        }
        if (col is CapsuleCollider capsuleCollider) {
            Debug.Log("ER");
            // Perform the capsule overlap check
            numHit = Physics.OverlapCapsuleNonAlloc(capsulePoint1.position, capsulePoint2.position, .4f, hitColliders);
        }

        for (int i = 0; i < numHit; i++) {
            OnTriggerEnter(hitColliders[i]);
        }
        
        if(reset)
         Reset();
    }
}
