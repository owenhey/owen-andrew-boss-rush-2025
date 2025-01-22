using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Sound : MonoBehaviour {
    public static Sound I;

    public Sounder Footstepsound;

    public void Awake() {
        I = this;
    }

    public void PlayFootstep(float delay = 0) {
        Footstepsound.Play(delay);
    }
}
