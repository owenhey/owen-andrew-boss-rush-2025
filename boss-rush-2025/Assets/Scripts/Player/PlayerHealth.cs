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

    private void Awake() {
        currentHealth = MaxHealth;
    }

    public void TakeDamage(float damage, Transform source) {
        movement.Knockback(source);

        TextPopups.Instance.Get().PopupAbove(damage.ToString(), movement.transform.position + Vector3.up, .3f).SetColor(Color.red);

        currentHealth -= damage;
        OnTakeDamage?.Invoke(-damage, currentHealth);
        if (currentHealth <= 0) {
            Die();
        }

        ScreenShake();
    }

    private void Die() {
        OnDie?.Invoke();
    }

    private void ScreenShake() {
        var noise = GameObject.FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();
        shakeTween?.Kill();
        shakeTween = DOTween.To(() => noise.AmplitudeGain, (x) => noise.AmplitudeGain = x, 0, shakeDuration)
            .From(shakeAmp);
    }
}
