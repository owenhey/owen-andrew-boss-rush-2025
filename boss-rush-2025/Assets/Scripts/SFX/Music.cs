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

    public float musicVolume = .5f;

    public AudioSource Boss2Source;

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
            Boss2Source.clip = SpiderTheme.track;
        }

        if (s == "blob") {
            Play(BlobThemeIntro);
            Boss2Source.clip = BlobTheme.track;
        }
        
        if (s == "robot") {
            Play(RobotThemeIntro);
            Boss2Source.clip = RobotTheme.track;
        }
    }

    public void FadeOut() {
        Boss2Source.DOFade(0, fadeTime);
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
        MusicSource.DOFade(0, 3);

        Boss2Source.time = currentTime;
        Boss2Source.DOFade(musicVolume, 3).From(0);
        Boss2Source.Play();
    }

    private void Play(MusicData musicData) {
        if (MusicSource.isPlaying) {
            MusicSource.DOFade(0, fadeTime).OnComplete(() => {
                MusicSource.clip = musicData.track;
                MusicSource.Play();
                MusicSource.DOFade(musicVolume, fadeTime).From(0);
            });
        }
        else {
            MusicSource.clip = musicData.track;
            MusicSource.Play();
            MusicSource.DOFade(musicVolume, fadeTime).From(0);
        }
    }
}
