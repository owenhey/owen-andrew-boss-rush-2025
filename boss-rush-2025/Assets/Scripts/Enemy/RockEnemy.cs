using DG.Tweening;
using UnityEngine;

public class RockEnemy : Enemy {
    public RockBehavior RockBehavior;

    private void Start() {
        transform.DOScale(.43f, .35f).SetEase(Ease.OutElastic).From(0);
        ShouldMove = false;
        cc.enabled = false;
        transform.DOJump(transform.position, 2, 1, .35f).SetEase(Ease.Linear).OnComplete(() => {
            ShouldMove = true;
            cc.enabled = true;
        });    
    }
    
    protected override void Die() {
        RockBehavior.enabled = true;
        RockBehavior.Go();
        Destroy(this);
        Destroy(this.cc);
    }
}
