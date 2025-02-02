using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHealthBar : MonoBehaviour {
    public RectTransform HealthBarRt;
    public RectTransform DelayHealthBarRT;
    public RectTransform HealthBarParent;
    public Image healthBarImage;
    public TextMeshProUGUI bossNameField;
    public CanvasGroup cg;

    public Color goodColor;
    public Color badColor;

    public static BossHealthBar instance;

    public Enemy currentBoss = null;

    private void Awake() {
        instance = this;
        cg.alpha = 0;
    }

    public void Setup(Enemy e) {
        Debug.Log("setting up");
        currentBoss = e;
        currentBoss.OnDie += EnemyDie;
        currentBoss.OnChangeHealth += OnHealthChangeHandler;
        bossNameField.text = e.EnemyName;
        
        cg.DOFade(1.0f, .5f).From(0);
        
        UpdateHeath(10000000, false);
        UpdateHeath(e.CurrentHealth, true);
    }

    public void EnemyDie() {
        currentBoss.OnDie -= EnemyDie;
        currentBoss.OnChangeHealth -= OnHealthChangeHandler;

        cg.DOFade(0.0f, .5f);
        currentBoss = null;
    }

    private void OnHealthChangeHandler(float delta, float newHealth) {
        UpdateHeath(newHealth, true);
        if (delta < 0) {
            Debug.Log("Shaking");
            HealthBarParent.anchoredPosition = new Vector2(0, -75);
            HealthBarParent.DOShakeAnchorPos(.2f, 30, 40);
        }
    }

    private void UpdateHeath(float health, bool animate) {
        if (currentBoss == null) {
            Debug.LogWarning("null health on boss");
        }
        
        float t = Mathf.Clamp01(health / currentBoss.maxHealth);
        float totalSize = HealthBarParent.sizeDelta.x - 20;

        cg.alpha = t > .999f ? 0.0f : 1.0f;

        healthBarImage.color = Color.Lerp(badColor, goodColor, t);

        if (animate) {
            HealthBarRt.DOKill();
            HealthBarRt.DOSizeDelta(new Vector2(totalSize * t, HealthBarRt.sizeDelta.y), .25f).SetEase(Ease.OutQuad);

            DelayHealthBarRT.DOKill();
            DelayHealthBarRT.DOSizeDelta(new Vector2(totalSize * t, DelayHealthBarRT.sizeDelta.y), .25f).SetDelay(.25f)
                .SetEase(Ease.OutQuad);
        }
        else {
            HealthBarRt.sizeDelta = new Vector2(totalSize * t, HealthBarRt.sizeDelta.y);
            DelayHealthBarRT.sizeDelta = new Vector2(totalSize * t, DelayHealthBarRT.sizeDelta.y);
        }
    }
}
