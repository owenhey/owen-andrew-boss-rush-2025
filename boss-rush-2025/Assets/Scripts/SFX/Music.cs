using DG.Tweening;
using UnityEngine;

public class Music : MonoBehaviour {
    [System.Serializable]
    private struct MusicData {
        public AudioClip track;
        public float volume;
    }
    
    public static Music I;
    public AudioSource MusicSource;

    public float fadeTime = .5f;
    
    [SerializeField] private MusicData HubTheme;
    [SerializeField] private MusicData BlobTheme;
    [SerializeField] private MusicData SpiderTheme;

    private void Awake() {
        I = this;
        Play(HubTheme);
    }

    public void PlayString(string s) {
        if (s == "spider") {
            Play(SpiderTheme);
        }

        if (s == "blob") {
            Play(BlobTheme);
        }
    }

    public void FadeOut() {
        MusicSource.DOFade(0, fadeTime);
    }

    private void Play(MusicData musicData) {
        if (MusicSource.isPlaying) {
            MusicSource.DOFade(0, fadeTime).OnComplete(() => {
                MusicSource.clip = musicData.track;
                MusicSource.Play();
                MusicSource.DOFade(musicData.volume, fadeTime).From(0);
            });
        }
        else {
            MusicSource.clip = musicData.track;
            MusicSource.Play();
            MusicSource.DOFade(musicData.volume, fadeTime).From(0);
        }
    }
}
