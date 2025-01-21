using UnityEngine;

public class BossAreaTrigger : MonoBehaviour {
    private bool trigger = false;

    public Enemy boss;

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 7) {
            if (trigger) return;
            trigger = true;
            boss.HandlePlayerEnterBossZone();
        }
    }
}
