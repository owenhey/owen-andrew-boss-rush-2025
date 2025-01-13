using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable {
    public float knockBackFactor = 1.0f;
    public CharacterController cc;

    private Vector3 knockbackTarget;
    protected bool knockedBack = false;

    protected Vector3 targetPosition;
    public Transform transformTarget;

    public bool InCombat = false;
    protected bool ShouldMove = true;

    [Header("Stats")] 
    public float speed;
    public float damping;
    private Vector3 vel;

    protected Movement player;

    [Header("Flavor")] [SerializeField] 
    [TextArea(3, 3)] private string combatStartText;
    [TextArea(3, 3)] private string playerStartCombatText;

    private Tween currentKnockback;

    protected virtual void Awake() {
        transformTarget.parent = null;
        player = FindFirstObjectByType<Movement>();
        targetPosition = cc.transform.position;
    }

    private void Update() {
        if (!knockedBack && ShouldMove) {
            Move();
        }

        OnUpdate();
    }

    protected virtual void OnUpdate() { }
    
    protected virtual void Move() {
        transformTarget.position = Vector3.SmoothDamp(cc.transform.position, targetPosition, ref vel, damping);

        Vector3 towards = (transformTarget.position - cc.transform.position);
        if (towards != Vector3.zero) {
            towards.Normalize();
            towards *= speed * Time.deltaTime;
            towards.y = -1;

            cc.Move(towards);
        }
        
    }
    
    public virtual void TakeDamage(float damage, Transform source) {
        if (knockBackFactor != 0.0) {
            knockedBack = true;
            Vector3 direction = cc.transform.position - source.position;
            direction.y = 0;
            direction.Normalize();

            if (!InCombat) {
                InCombat = true;
                TextPopups.Instance.Get().PopupAbove("Die!!!", player.transform, 1.0f);
                TextPopups.Instance.Get().PopupAbove(combatStartText, transform, 2.0f, .5f);
                HandleCombatStart();
            }
        
            knockbackTarget = knockBackFactor * CalcKnockback(damage) * direction + cc.transform.position;
            TextPopups.Instance.Get().PopupAbove(damage.ToString(), Vector3.Lerp(cc.transform.position, knockbackTarget, .5f), .5f);
            
            Knockback();
        }
    }

    protected virtual void HandleCombatStart() {
        
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
