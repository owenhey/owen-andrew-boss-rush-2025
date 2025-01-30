using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class LoveInterest : Enemy {
    public Transform ikTarget;

    private float NextAttackTime = 10000;

    public Transform attackStart;
    public Transform attackEnd;

    public float attackDuration = .35f;

    private bool swinging = false;
    private float swingStart = 0.0f;
    private float swingEnd = 0.0f;

    private void Start() {
        NextAttackTime = Time.time + 2.0f;
    }

    protected override void OnUpdate() {
        base.OnUpdate();

        if (Time.time > NextAttackTime) {
            StartCoroutine(Swing());
        }

        if (InCombat) {
            if (swinging) {
                if (Time.time > swingEnd) {
                    swinging = false;
                    ikTarget.DOLocalMove(GetLocalSwingPosition(0), .3f);
                }

                float t = 1 - ((swingEnd - Time.time) / attackDuration);
                
                ikTarget.localPosition = GetLocalSwingPosition(t);
            }
        }
    }

    private Vector3 GetLocalSwingPosition(float t) {
        t = t * t; // in quad

        float remappedT = RemapClamp(t, 0, 1, .1f, .9f);

        float ySubtract = Mathf.Sin(Mathf.PI * t) * -.25f;
                
        Vector3 localPos = new Vector3(
            Mathf.Cos(Mathf.PI * remappedT),
            attackStart.localPosition.y + ySubtract,
            Mathf.Sin(Mathf.PI * remappedT)
        );

        return localPos;
    }

    private IEnumerator Swing() {
        NextAttackTime = Time.time + 2.0f;

        yield return null;
        swingStart = Time.time;
        swingEnd = Time.time + attackDuration;
        swinging = true;
    }

    /// <summary>
    /// Remaps a value from one range into another using linear interpolation. Clamps the starting values
    /// </summary>
    public static float RemapClamp(float value, float startLow, float startHigh, float endLow, float endHigh) {
        value = Mathf.Clamp(value, startLow, startHigh);
        return endLow + ((endHigh - endLow) / (startHigh - startLow)) * (value - startLow);
    }
}
