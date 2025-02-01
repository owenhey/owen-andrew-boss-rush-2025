using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public class Sounder : MonoBehaviour {
    public float Volume = 1;
    
    private AudioSource[] _sources;

    public bool loop = false;

    public List<AudioClip> sources;

    private void Start() {
        if (sources != null) {
            foreach (var source in sources) {
                var newSource = gameObject.AddComponent<AudioSource>();
                newSource.clip = source;
                newSource.outputAudioMixerGroup = GameManager.instance.mixer;
            }
        }
        _sources = GetComponentsInChildren<AudioSource>();
        foreach (var source in _sources) {
            source.loop = loop;
            source.playOnAwake = false;
            source.Stop();
            source.volume *= Volume;
            source.outputAudioMixerGroup = GameManager.instance.mixer;
        }
    }

    public void PlayLoop() {
        for (int i = 0; i < _sources.Length; i++) {
            if (_sources[i].isPlaying) return;
        }
        _sources[Random.Range(0, _sources.Length)].Play();
    }

    public void Play(float delay) {
        int startindex = Random.Range(0, _sources.Length);
        for (int i = 0; i < _sources.Length; i++) {
            if(_sources[(i + startindex) % _sources.Length].isPlaying) continue;
            
            _sources[(i + startindex) % _sources.Length].pitch = Random.Range(.9f, 1.1f);
            _sources[(i + startindex) % _sources.Length].time = 0;
            _sources[(i + startindex) % _sources.Length].PlayDelayed(delay);
            
            return;
        }

        _sources[startindex].pitch = Random.Range(.9f, 1.1f);
        _sources[startindex].time = 0;
        _sources[startindex].PlayDelayed(delay);
    }

    public void Stop() {
        for (int i = 0; i < _sources.Length; i++) {
            _sources[i].Stop();
        }
    }
}
