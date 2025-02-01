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
    [SerializeField] private MusicData BlobThemeIntro;
    [SerializeField] private MusicData SpiderTheme;
    [SerializeField] private MusicData SpiderThemeIntro;
    [SerializeField] private MusicData RobotTheme;
    [SerializeField] private MusicData RobotThemeIntro;

    private void Awake() {
        I = this;
        PlayHub();
    }

    public void PlayHub() {
        Play(HubTheme);
    }

    public void PlayStringIntro(string s) {
        if (s == "spider") {
            Play(SpiderThemeIntro);
        }

        if (s == "blob") {
            Play(BlobThemeIntro);
        }
        
        if (s == "robot") {
            Play(RobotThemeIntro);
        }
    }

    public void FadeOut() {
        MusicSource.DOFade(0, fadeTime);
    }

    public void SwitchToActualTheme(string bossName) {
        float currentTime = MusicSource.time;

        AudioClip ac = SpiderTheme.track;
        if (bossName == "spider") {
            ac = SpiderTheme.track;
        }

        else if (bossName == "blob") {
            ac = BlobTheme.track;
        }
        
        if (bossName == "robot") {
            ac = RobotTheme.track;
        }

        MusicSource.clip = ac;
        MusicSource.time = currentTime;
        MusicSource.Play();
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
