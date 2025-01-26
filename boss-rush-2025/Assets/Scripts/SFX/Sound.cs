using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Sound : MonoBehaviour {
    public static Sound I;

    public Sounder Footstepsound;
    public Sounder Swing1;
    public Sounder Swing2;
    public Sounder Hit;

    public void Awake() {
        I = this;
    }

    public void PlayFootstep(float delay = 0) {
        Footstepsound.Play(delay);
    }
    
    public void PlaySwing1(float delay = 0) {
        Swing1.Play(delay);
    }
    
    public void PlaySwing2(float delay = 0) {
        Swing2.Play(delay);
    }
    
    public void PlayHit(float delay = 0) {
        Hit.Play(delay);
    }
}
