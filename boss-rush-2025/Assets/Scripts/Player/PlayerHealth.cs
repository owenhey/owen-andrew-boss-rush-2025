using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

namespace Player {
    public class PlayerHealth : MonoBehaviour, IDamagable {
        public Movement movement;

        public float shakeDuration;
        public float shakeAmp;
        private Tween shakeTween = null;
        public void TakeDamage(float damage, Transform source) {
            movement.Knockback(source);

            ScreenShake();
        }

        private void ScreenShake() {
            var noise = GameObject.FindFirstObjectByType<CinemachineBasicMultiChannelPerlin>();
            shakeTween?.Kill();
            shakeTween = DOTween.To(() => noise.AmplitudeGain, (x) => noise.AmplitudeGain = x, 0, shakeDuration)
                .From(shakeAmp);
        }
    }
}