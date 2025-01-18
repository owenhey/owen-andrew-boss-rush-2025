using System;
using DG.Tweening;
using UnityEngine;

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
        transformTarget.position = Vector3.SmoothDamp(cc.transform.position, targetPosition, ref vel, damping, speed);

        Vector3 towards = (transformTarget.position - cc.transform.position);
        if (towards != Vector3.zero) {
            towards.y = -1;
            cc.Move(towards);
        }
    }
    
    public virtual void TakeDamage(float damage, Transform source) {
        if (knockBackFactor * knockbackFactorCode != 0.0) {
            knockedBack = true;
            Vector3 direction = cc.transform.position - source.position;
            direction.y = 0;
            direction.Normalize();

            var inBetween = (source.transform.position + transform.position) * .5f;
            inBetween += Vector3.up;
            
            SplatManager.Instance.Get().Setup(inBetween, hitColor);

            if (!InCombat) {
                InCombat = true;
                TextPopups.Instance.Get().PopupAbove("Die!!!", player.transform, 1.0f);
                TextPopups.Instance.Get().PopupAbove(combatStartText, transform, 2.0f, .5f);
                HandleCombatStart();
            }
        
            knockbackTarget = knockbackFactorCode * knockBackFactor * CalcKnockback(damage) * direction + cc.transform.position;
            TextPopups.Instance.Get().PopupAbove(damage.ToString(), Vector3.Lerp(cc.transform.position, knockbackTarget, .5f), .5f);
            
            Knockback();
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

    private void Knockback() {
        currentKnockback?.Kill();
        Vector3 x = cc.transform.position;
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
