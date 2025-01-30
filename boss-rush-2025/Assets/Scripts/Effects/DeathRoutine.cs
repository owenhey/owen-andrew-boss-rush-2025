using System;
using DG.Tweening;
using UnityEngine;

public class DeathRoutine : MonoBehaviour {
    public CanvasGroup CG;
    public float DelayTime = 2.0f;

    public static bool DiedBefore;

    private void Awake() {
        CG.gameObject.SetActive(true);
        CG.DOFade(0, 1).From(1).SetDelay(.5f).OnComplete(() => {
            CG.gameObject.SetActive(false);
        });
    }

    private void OnEnable() {
        PlayerHealth.OnDie += HandleDeath;
    }

    private void OnDisable() {
        PlayerHealth.OnDie -= HandleDeath;
    }
    
    private void HandleDeath() {
        CG.gameObject.SetActive(true);
        CG.DOFade(1, 1).From(0).SetDelay(DelayTime).OnStart(() => {
            DiedBefore = true;
        });
    }
}
