using System;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class AboveHealthBar : MonoBehaviour {
    public Vector3 distanceFrom = Vector3.up * 3;
    public Enemy enemy;
    public RectTransform HealthBarRt;
    public RectTransform DelayHealthBarRT;
    public RectTransform HealthBarParent;
    public Image healthBarImage;
    public CanvasGroup cg;

    private Transform cam;

    public Color goodColor;
    public Color badColor;
    

    private void Start() {
        transform.SetParent(null, false);
        cam = Camera.main.transform;

        float parentHealth = enemy.maxHealth;

        float size = Mathf.Clamp(parentHealth * 5, 300.0f, 700);
        
        HealthBarParent.sizeDelta = new Vector2(size, HealthBarParent.sizeDelta.y);
        
        UpdateHeath(enemy.maxHealth, false);
        
        enemy.OnDie += OnDie;
    }

    private void OnDie() {
        Destroy(gameObject);
    }

    private void Update() {
        var awayFromCamera = transform.position - cam.transform.position;
        transform.forward = awayFromCamera;

        transform.position = enemy.transform.position + distanceFrom;
    }

    private void OnEnable() {
        enemy.OnChangeHealth += OnHealthChangeHandler;
    }

    private void OnDisable() {
        enemy.OnChangeHealth -= OnHealthChangeHandler;
    }

    private void OnDestroy() {
        HealthBarRt.DOKill();
        DelayHealthBarRT.DOKill();
    }

    private void OnHealthChangeHandler(float delta, float newHealth) {
        UpdateHeath(newHealth, true);
        if (delta < 0) {
            HealthBarParent.DOShakeAnchorPos(.3f, 50, 50);
        }
    }

    private void UpdateHeath(float health, bool animate) {
        float t = Mathf.Clamp01(health / enemy.maxHealth);
        float totalSize = HealthBarParent.sizeDelta.x - 20;

        cg.alpha = t > .999f ? 0.0f : 1.0f;

        healthBarImage.color = Color.Lerp(badColor, goodColor, t);

        if (animate) {
            HealthBarRt.DOKill();
            HealthBarRt.DOSizeDelta(new Vector2(totalSize * t, HealthBarRt.sizeDelta.y), .25f).SetEase(Ease.OutQuad);
            
            DelayHealthBarRT.DOKill();
            DelayHealthBarRT.DOSizeDelta(new Vector2(totalSize * t, DelayHealthBarRT.sizeDelta.y), .25f).SetDelay(.25f).SetEase(Ease.OutQuad);
        }
        else {
            HealthBarRt.sizeDelta = new Vector2(totalSize * t, HealthBarRt.sizeDelta.y);
            DelayHealthBarRT.sizeDelta = new Vector2(totalSize * t, DelayHealthBarRT.sizeDelta.y);
        }
    }
}
