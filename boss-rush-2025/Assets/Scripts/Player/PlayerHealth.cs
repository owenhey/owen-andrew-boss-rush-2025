using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable {
    public Movement movement;

    public float shakeDuration;
    public float shakeAmp;
    private Tween shakeTween = null;

    public Action<float, float> OnTakeDamage;
    public static Action OnDie;

    public float MaxHealth;
    private float currentHealth;
    public float CurrentHealth => currentHealth;

    private bool dead = false;

    private void Awake() {
        currentHealth = MaxHealth;
    }

    private void Update() {
        if (dead) return;
        if (Input.GetKeyDown(KeyCode.K)) {
            TakeDamage(1000, transform);
        }
    }

    public void TakeDamage(float damage, Transform source, bool force = false) {
        if (dead) return;
        
        movement.Knockback(source);

        Vector3 lerped = Vector3.Lerp(movement.transform.position + Vector3.up, source.position, .5f);

        TextPopups.Instance.Get().PopupAbove(damage.ToString(), lerped, .3f).SetColor(Color.red);

        currentHealth -= damage;
        OnTakeDamage?.Invoke(-damage, currentHealth);
        if (currentHealth <= 0) {
            Die(movement.transform.position - source.position);
        }

        ScreenShake();
    }

    private void Die(Vector3 direction) {
        if (dead) return;
        
        dead = true;
        OnDie?.Invoke();
        movement.BlowUp(direction.normalized);
    }

    private void ScreenShake() {
        if (dead) return;
        
        var noise = GameObject.FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();
        shakeTween?.Kill();
        shakeTween = DOTween.To(() => noise.AmplitudeGain, (x) => noise.AmplitudeGain = x, 0, shakeDuration)
            .From(shakeAmp);
    }
}
