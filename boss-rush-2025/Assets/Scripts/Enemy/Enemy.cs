using DG.Tweening;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamagable {
    public float knockBackFactor = 1.0f;
    public CharacterController cc;

    protected bool knockedBack = false;

    private Vector3 knockbackTarget;

    private void Update() {
        if (knockedBack) {
            knockedBack = false;

            Vector3 x = cc.transform.position;
            DOTween.To(() => x, (y) => {
                var dV = y - cc.transform.position;
                cc.Move(dV);
            }, knockbackTarget, .3f * knockBackFactor).SetEase(Ease.OutQuad);
        }
        else {
            Move();
        }
        
        OnUpdate();
    }

    protected virtual void OnUpdate() { }
    
    private void Move() {
        
    }
    
    public void TakeDamage(float damage, Transform source) {
        if (knockBackFactor != 0.0) {
            knockedBack = true;
            Vector3 direction = cc.transform.position - source.position;
            direction.y = 0;
            direction.Normalize();
        
            knockbackTarget = knockBackFactor * CalcKnockback(damage) * direction + cc.transform.position;
            TextPopups.Instance.Get().PopupAbove(damage.ToString(), Vector3.Lerp(cc.transform.position, knockbackTarget, .5f), .5f);
        }
    }

    private float CalcKnockback(float damage) {
        return Mathf.Max(.5f, damage / 10);
    }
}
