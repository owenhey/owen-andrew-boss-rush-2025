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

    protected Vector3 knockbackTarget;
    protected bool knockedBack = false;

    public bool Resettargetonknockback = true;

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

    public Color hitColor = Color.white;

    public Action<float, float> OnChangeHealth;
    public Action OnDie;

    private Tween currentKnockback;

    protected Vector3 velTarget;
    private Vector3 velVel;

    public string musicString;

    [Header("Flavor")] [SerializeField] 
    [TextArea(2,2)]
    public List<string> possibleEnterBossZoneMessages;
    
    [TextArea(2,2)]
    public List<string> possiblePlayerKilledMessages;
    
    [TextArea(2,2)]
    public List<string> possiblePlayerStartCombatMessages;
    
    [TextArea(2,2)]
    public List<string> possibleStartCombatLines;

    protected bool canBeDamaged = true;
    
    [Header("During combat player lines")]
    [TextArea(2,2)]
    public List<string> possiblePlayerDuringCombatLines;
    [TextArea(2,2)]
    public List<string> possibleEnemyTalkDuringCombatLines;

    private float firstTalkThreshold;
    private float secondTalkThreshold;
    private float thirdTalkThreshold;
    private float fourthTalkThreshold;

    private int talkDuringCombatAmount = 0;

    public bool DestroyOnDeath = true;
    
    protected virtual void Awake() {
        transformTarget.parent = null;
        player = FindFirstObjectByType<Movement>();
        targetPosition = cc.transform.position;
        currentHealth = maxHealth;

        firstTalkThreshold = Random.Range(.7f, .8f);
        secondTalkThreshold = Random.Range(.55f, .65f);
        thirdTalkThreshold = Random.Range(.4f, .5f);
        fourthTalkThreshold = Random.Range(.2f, .3f);
        
        OnAwake();
    }

    protected virtual void OnAwake() {
        
    }

    private void Update() {
        if (currentHealth < 0) return;
        
        OnUpdate();
        if (!knockedBack && ShouldMove) {
            Move();
        }
    }

    protected virtual void OnUpdate() { }
    
    protected virtual void Move() {
        transformTarget.position = Vector3.SmoothDamp(cc.transform.position, targetPosition, ref vel, damping, speed * speedFactor);
        
        velTarget = Vector3.SmoothDamp(velTarget, (targetPosition - transform.position), ref velVel, damping, speed * speedFactor);

        Vector3 towards = (transformTarget.position - cc.transform.position);
        if (towards != Vector3.zero) {
            towards.y = -1;
            cc.Move(towards);
        }
    }
    
    public virtual void TakeDamage(float damage, Transform source, bool force = false) {
        if(!force)
            if (!canBeDamaged) return;
        
        Vector3 direction = cc.transform.position - source.position;
        direction.y = 0;
        direction.Normalize();
        Vector3 inBetween = transform.position + Vector3.up;
        damageTowards = direction;
        if (knockBackFactor * knockbackFactorCode != 0.0) {
            knockedBack = true;

            inBetween = (source.transform.position + transform.position) * .5f;
            inBetween += Vector3.up;
        
            knockbackTarget = knockbackFactorCode * knockBackFactor * CalcKnockback(damage) * direction + cc.transform.position;
            knockbackTarget.y = transform.position.y;
            
            Knockback();
        }

        string text = damage.ToString("N0");
        if (damage < 0) {
            text = text.Substring(1, text.Length - 1);
        }
        var popup = TextPopups.Instance.Get().PopupAbove(text, inBetween, .5f);
        if (damage < 0) 
            popup.SetColor(Color.green);
        
        SplatManager.Instance.Get().Setup(inBetween, hitColor);

        if (!InCombat) {
            InCombat = true;
            HandleCombatStart();
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, -1, maxHealth);
        OnChangeHealth?.Invoke(-damage, currentHealth);
        if (currentHealth <= 0) {
            Die();
        }
        else {
            float percentHealth = currentHealth / maxHealth;
            if (talkDuringCombatAmount == 0 && percentHealth < firstTalkThreshold) {
                talkDuringCombatAmount++;
                if (possibleEnemyTalkDuringCombatLines != null && possibleEnemyTalkDuringCombatLines.Count > 0) {
                    var randomString = possibleEnemyTalkDuringCombatLines[Random.Range(0, possibleEnemyTalkDuringCombatLines.Count)];
                    float wait = Random.Range(.5f, 1.0f);
                    TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.5f, wait).MakePopout();
                    OnEnemySpeak(wait);
                    possibleEnemyTalkDuringCombatLines.Remove(randomString);
                }
            }
            
            if (talkDuringCombatAmount == 1 && percentHealth < secondTalkThreshold) {
                talkDuringCombatAmount++;
                if (possiblePlayerDuringCombatLines != null && possiblePlayerDuringCombatLines.Count > 0) {
                    var randomString = possiblePlayerDuringCombatLines[Random.Range(0, possiblePlayerDuringCombatLines.Count)];
                    float wait = Random.Range(.5f, 1.0f);
                    TextPopups.Instance.Get().PopupAbove(randomString, player.transform, 2.5f, wait).MakePopout();

                    possiblePlayerDuringCombatLines.Remove(randomString);
                }
            }
            
            if (talkDuringCombatAmount == 2 && percentHealth < thirdTalkThreshold) {
                talkDuringCombatAmount++;
                if (possibleEnemyTalkDuringCombatLines != null && possibleEnemyTalkDuringCombatLines.Count > 0) {
                    var randomString = possibleEnemyTalkDuringCombatLines[Random.Range(0, possibleEnemyTalkDuringCombatLines.Count)];
                    float wait = Random.Range(.5f, 1.0f);
                    TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.5f, wait).MakePopout();
                    OnEnemySpeak(wait);
                    possibleEnemyTalkDuringCombatLines.Remove(randomString);
                }
            }
            
            if (talkDuringCombatAmount == 3 && percentHealth < fourthTalkThreshold) {
                talkDuringCombatAmount++;
                if (possiblePlayerDuringCombatLines != null && possiblePlayerDuringCombatLines.Count > 0) {
                    var randomString = possiblePlayerDuringCombatLines[Random.Range(0, possiblePlayerDuringCombatLines.Count)];
                    float wait = Random.Range(.5f, 1.0f);
                    TextPopups.Instance.Get().PopupAbove(randomString, player.transform, 2.5f, wait).MakePopout();

                    possiblePlayerDuringCombatLines.Remove(randomString);
                }
            }
        }
    }

    public void Kill() {
        currentHealth = 0;
        OnChangeHealth(-100000, 0);
        Die();
    }

    public BossDieRoutine bossDeathRoutine;

    protected Vector3 damageTowards;

    protected virtual void OnEnemySpeak(float delay) {
        
    }
    
    protected virtual void Die() {
        Destroy(transformTarget.gameObject);
        if (DestroyOnDeath) {
            Destroy(gameObject);
        }
        else {
            InCombat = false;
            canBeDamaged = false;
            ShouldMove = false;
        }

        OnDie?.Invoke();

        if (useBossHealthBar) {
            bossDeathRoutine.BossKillRoutine(EnemyName);
            GameManager.lastBossKilled = EnemyName;
        }
    }

    protected virtual void HandleCombatStart() {
        HandlePlayerFirstAttack();
        PlayBossEnterCombatLines();

        if (!string.IsNullOrEmpty(musicString)) {
            Music.I.SwitchToActualTheme(musicString);
        }
        
        ShowHealthBar();
    }

    public void ShowHealthBar() {
        if (useBossHealthBar) {
            BossHealthBar.instance.Setup(this);
        }
    }

    public virtual void HandlePlayerEnterBossZone() {
        if (possibleEnterBossZoneMessages == null || possibleEnterBossZoneMessages.Count == 0) return;

        var randomString = possibleEnterBossZoneMessages[Random.Range(0, possibleEnterBossZoneMessages.Count)];
        OnEnemySpeak(0);
        TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.0f);
    }

    public virtual void HandlePlayerFirstAttack() {
        if (possiblePlayerStartCombatMessages == null || possiblePlayerStartCombatMessages.Count == 0) return;

        var randomString = possiblePlayerStartCombatMessages[Random.Range(0, possiblePlayerStartCombatMessages.Count)];
        TextPopups.Instance.Get().PopupAbove(randomString, player.transform, 1.5f);
    }
    
    public virtual void PlayBossEnterCombatLines() {
        if (possibleStartCombatLines == null || possibleStartCombatLines.Count == 0) return;

        var randomString = possibleStartCombatLines[Random.Range(0, possibleStartCombatLines.Count)];
        OnEnemySpeak(.7f);
        TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.0f, .7f);
    }

    private void Knockback() {
        currentKnockback?.Kill();
        Vector3 x = cc.transform.position;

        if (Resettargetonknockback) {
            targetPosition = knockbackTarget;
            transformTarget.position = knockbackTarget;
        }
        vel = Vector3.zero;
        
        
        
        currentKnockback = DOTween.To(() => x, (y) => {
            var dV = y - cc.transform.position;
            cc.Move(dV);
        }, knockbackTarget, .4f * knockBackFactor * knockbackFactorCode).SetEase(Ease.OutQuad).OnComplete(() => {
            knockedBack = false;
        });
    }

    private float CalcKnockback(float damage) {
        return Mathf.Max(.5f, damage / 10);
    }

    public void SayPlayerDeathText() {
        if (possiblePlayerKilledMessages == null || possiblePlayerKilledMessages.Count == 0) return;
        OnEnemySpeak(.5f);
        var randomString = possiblePlayerKilledMessages[Random.Range(0, possiblePlayerKilledMessages.Count)];
        TextPopups.Instance.Get().PopupAbove(randomString, transform, 2.5f, .5f);
    }
}
