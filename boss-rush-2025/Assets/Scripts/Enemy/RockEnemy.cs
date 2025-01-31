using UnityEngine;

public class RockEnemy : Enemy {
    public RockBehavior RockBehavior;
    protected override void Die() {
        RockBehavior.enabled = true;
        RockBehavior.Go();
        Destroy(this);
        Destroy(this.cc);
    }
}
