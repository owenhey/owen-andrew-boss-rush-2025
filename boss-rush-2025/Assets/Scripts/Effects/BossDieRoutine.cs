using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class BossDieRoutine : MonoBehaviour {
    public DeathRoutine deathRoutine;

    public TextMeshProUGUI text;
    public TextMeshProUGUI text2;
    
    public void BossKillRoutine(string bossName) {
        StartCoroutine(BossC(bossName));
    }

    private IEnumerator BossC(string bossname) {
        yield return new WaitForSeconds(1.5f);
        Sound.I.PlayBossDefeated();
        yield return new WaitForSeconds(.5f);
        Music.I.FadeOut();
        GameManager.instance.EnableCutscene();

        text.gameObject.SetActive(true);
        text2.gameObject.SetActive(true);

        text.text = "Boss Defeated:";
        text2.text = bossname;

        text.DOFade(1.0f, 1.0f).From(0).SetDelay(2.0f);
        text2.DOFade(1.0f, 1.0f).From(0).SetDelay(3.0f);
        
        deathRoutine.CG.gameObject.SetActive(true);
        deathRoutine.CG.DOFade(1.0f, 1.0f).OnComplete(() => {
            deathRoutine.CG.DOFade(1.0f, 6f).OnComplete(() => {
                text2.DOFade(0.0f, 1.0f);
                text.DOFade(0.0f, 1.0f).OnComplete(() => {
                    GameManager.instance.Restart();
                });
            });
        });
    }
}
