using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour, IDamagable {
    public string EnemyName;
    public float knockBackFactor = 1.0f;
    protected float knockbackFactorCode = 1.0f;
    public CharacterController cc;

    private Vector3 knockbackTarget;
    protected bool knockedBack = false;

    protected Vector3 targetPosition;
    public Transform transformTarget;

    public bool InCombat = false;
    protected bool ShouldMove = true;

    [Header("Stats")] 
    public float maxHealth = 35;
    public float speed;
    protected float speedFactor = 1.0f;
    public float damping;
    private Vector3 vel;

    [Header("boss health bar")] 
    public bool useBossHealthBar;

    private float currentHealth = 100;
    public float CurrentHealth => currentHealth;

    protected Movement player;

    [Header("Flavor")] [SerializeField] 
    [TextArea(3, 3)] private string combatStartText;
    [TextArea(3, 3)] private string playerStartCombatText;
    public Color hitColor = Color.white;

    public Action<float, float> OnChangeHealth;
    public Action OnDie;

    private Tween currentKnockback;

    [TextArea(2,2)]
    public List<string> possibleEnterBossZoneMessages;

    protected virtual void Awake() {
        transformTarget.parent = null;
        player = FindFirstObjectByType<Movement>();
        targetPosition = cc.transform.position;
        currentHealth = maxHealth;
    }

    private void Update() {
        OnUpdate();
        if (!knockedBack && ShouldMove) {
            Move();
        }
    }

    protected virtual void OnUpdate() { }
    
    protected virtual void Move() {
        transformTarget.position = Vector3.SmoothDamp(cc.transform.position, targetPosition, ref vel, damping, speed * speedFactor);

        Vector3 towards = (transformTarget.position - cc.transform.position);
        if (towards != Vector3.zero) {
            towards.y = -1;
            cc.Move(towards);
        }
    }
    
    public virtual void TakeDamage(float damage, Transform source) {
        Vector3 inBetween = transform.position + Vector3.up;
        if (knockBackFactor * knockbackFactorCode != 0.0) {
            knockedBack = true;
            Vector3 direction = cc.transform.position - source.position;
            direction.y = 0;
            direction.Normalize();

            inBetween = (source.transform.position + transform.position) * .5f;
            inBetween += Vector3.up;
        
            knockbackTarget = knockbackFactorCode * knockBackFactor * CalcKnockback(damage) * direction + cc.transform.position;
            
            Knockback();
        }
        
        TextPopups.Instance.Get().PopupAbove(damage.ToString(), inBetween, .5f);
        SplatManager.Instance.Get().Setup(inBetween, hitColor);

        if (!InCombat) {
            InCombat = true;
            TextPopups.Instance.Get().PopupAbove("Die!!!", player.transform, 1.0f);
            TextPopups.Instance.Get().PopupAbove(combatStartText, transform, 2.0f, .5f);
            HandleCombatStart();
        }

        currentHealth -= damage;
        OnChangeHealth?.Invoke(-damage, currentHealth);
        if (currentHealth <= 0) {
            Die();
        }
    }

    public void Kill() {
        currentHealth = 0;
        OnChangeHealth(-100000, 0);
        Die();
    }

    protected virtual void Die() {
        Destroy(transformTarget.gameObject);
        Destroy(gameObject);
        OnDie?.Invoke();
    }

    protected virtual void HandleCombatStart() {
        if (useBossHealthBar) {
            BossHealthBar.instance.Setup(this);
        }
    }

    public virtual void HandlePlayerEnterBossZone() {
        if (possibleEnterBossZoneMessages == null || possibleEnterBossZoneMessages.Count == 0) return;

        var randomString = possibleEnterBossZoneMessages[Random.Range(0, possibleEnterBossZoneMessages.Count)];
        TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.0f);
    }

    private void Knockback() {
        currentKnockback?.Kill();
        Vector3 x = cc.transform.position;
        targetPosition = knockbackTarget;
        currentKnockback = DOTween.To(() => x, (y) => {
            var dV = y - cc.transform.position;
            cc.Move(dV);
        }, knockbackTarget, .3f * knockBackFactor).SetEase(Ease.OutQuad).OnComplete(() => {
            knockedBack = false;
        });
    }

    private float CalcKnockback(float damage) {
        return Mathf.Max(.5f, damage / 10);
    }
}
