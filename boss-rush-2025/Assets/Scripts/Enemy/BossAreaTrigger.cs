using UnityEngine;

public class BossAreaTrigger : MonoBehaviour {
    private bool trigger = false;

    public Enemy boss;

    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8) {
            if (trigger) return;
            trigger = true;
            if(boss)
                boss.HandlePlayerEnterBossZone();
        }
    }
}
