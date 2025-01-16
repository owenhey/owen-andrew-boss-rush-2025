using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour {
    public RectTransform HealthBarRt;
    public RectTransform DelayHealthBarRT;
    public RectTransform HealthBarParent;
    public Image healthBarImage;

    public Color goodColor;
    public Color badColor;

    public PlayerHealth player;

    private void Start() {
        player.OnTakeDamage += OnHealthChangeHandler;
        
        UpdateHeath(100000, false);
    }

    private void OnHealthChangeHandler(float delta, float newHealth) {
        UpdateHeath(newHealth, true);
        
        
        
        if (delta < 0) {
            HealthBarParent.DOShakeAnchorPos(.2f, 30, 40);
        }
    }

    private void UpdateHeath(float health, bool animate) {
        float t = Mathf.Clamp01(health / player.MaxHealth);
        float totalSize = HealthBarParent.sizeDelta.x - 10;

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
