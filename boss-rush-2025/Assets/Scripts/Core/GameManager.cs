using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public AudioMixerGroup mixer;

    public PlayerInput input;

    private void Awake() {
        instance = this;

        PlayerHealth.OnDie += Reset;
    }

    private void OnDestroy() {
        PlayerHealth.OnDie -= Reset;
    }

    public void EnableGameplay() {
        Debug.Log("Enabling: Gameplay");
        input.SwitchCurrentActionMap("Player");
    }

    public void EnableUI() {
        Debug.Log("Enabling: UI");
        input.SwitchCurrentActionMap("UI");
    }

    public void Reset() {
        StartCoroutine(delayRestart());

        IEnumerator delayRestart() {
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(0);
        }
    }
}
