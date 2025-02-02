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
    public Sounder Laser1;
    public Sounder Laser2;
    public Sounder Roll;
    public Sounder Death;
    public Sounder SpiderTalk;
    public Sounder SpiderWalk;
    public Sounder SpiderAttack;
    public Sounder SpiderPreAttack;
    public Sounder RobotTalk;
    public Sounder RobotDeath;
    public Sounder BlobSquish;
    public Sounder BlobSquishLow;
    public Sounder PlayerHurt;
    public Sounder PlayerVoice;
    public Sounder LoveInterestVoice;
    public Sounder BossDefeated;

    public void Awake() {
        I = this;
    }

    public void PlayFootstep(float delay = 0) {
        Footstepsound.Play(delay);
    }
    
    public void PlaySpiderTalk(float delay = 0) {
        SpiderTalk.Play(delay);
    }

    public void PlaySpiderWalk() {
        SpiderWalk.PlayLoop();
    }

    public void StopSpiderWalk() {
        SpiderWalk.Stop();
    }
    
    public void PlaySwing1(float delay = 0) {
        Swing1.Play(delay);
    }
    
    public void PlaySwing2(float delay = 0) {
        Swing2.Play(delay);
    }
    
    public void PlayLaser1(float delay = 0) {
        Laser1.Play(delay);
    }
    
    public void PlayLaser2(float delay = 0) {
        Laser2.Play(delay);
    }
    
    public void PlayHit(float delay = 0) {
        Hit.Play(delay);
    }
    
    public void PlayRoll(float delay = 0) {
        Roll.Play(delay);
    }
    
    public void PlayDeath(float delay = 0) {
        Death.Play(delay);
    }
    
    public void PlaySpiderAttack(float delay = 0) {
        SpiderAttack.Play(delay);
    }
    
    public void PlaySpiderPreAttack(float delay = 0) {
        SpiderPreAttack.Play(delay);
    }
    
    public void PlayRobotTalk(float delay = 0) {
        RobotTalk.Play(delay);
    }
    
    public void PlayRobotDeath(float delay = 0) {
        RobotDeath.Play(delay);
    }
    
    public void PlayBlobSquish(float delay = 0) {
        BlobSquish.Play(delay);
    }
    
    public void PlayBlobSquishLow(float delay = 0) {
        BlobSquishLow.Play(delay);
    }
    
    public void PlayPlayerHurt(float delay = 0) {
        PlayerHurt.Play(delay);
    }
    
    public void PlayPlayerVoice(float delay = 0) {
        PlayerVoice.Play(delay);
    }
    
    public void PlayLoveInterestVoice(float delay = 0) {
        LoveInterestVoice.Play(delay);
    }
    
    public void PlayBossDefeated(float delay = 0) {
        BossDefeated.Play(delay);
    }
}
