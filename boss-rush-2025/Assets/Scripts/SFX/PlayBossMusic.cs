using UnityEngine;

public class PlayBossMusic : MonoBehaviour {
    public string bossName;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8) {
            Music.I.PlayStringIntro(bossName);
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.gameObject.layer == 7 || other.gameObject.layer == 8) {
            Music.I.PlayHub();
        }
    }
}
