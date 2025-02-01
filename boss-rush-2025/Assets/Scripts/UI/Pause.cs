using System;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Pause : MonoBehaviour {
    public InputActionReference openPause;

    public GameObject canvas;
    
    public AudioMixer mixer;
    public Selectable reset;

    public TextMeshProUGUI difficultySettingText;
    public TextMeshProUGUI aimTowardsMouseText;
    
    private void Awake() {
        openPause.action.performed += OpenPause;
        canvas.SetActive(false);
    }

    private void OnDestroy() {
        openPause.action.performed -= OpenPause;
    }

    public void OpenPause(InputAction.CallbackContext obj) {
        Time.timeScale = 0;
        canvas.SetActive(true);
        GameManager.instance.EnableUI();

        Debug.Log("Is controller?" + DetectInputMode.IsController);
        aimTowardsMouseText.gameObject.SetActive(!DetectInputMode.IsController);
        difficultySettingText.text = $"DIFFICULTY: {(GameManager.IsEasyMode ? "EASIER" : "REGULAR")}";
        aimTowardsMouseText.text = $"AIM TOWARDS MOUSE? {(GameManager.AimTowardsMouse ? "(ON)" : "(OFF)")}";
        reset.Select();
    }

    public void UpdateAudio(float f) {
        f /= 50;
        if (f < .5f) {
            f = LoveInterest.RemapClamp(f, 0, .5f, -80, 0);
        }
        else {
            f = LoveInterest.RemapClamp(f, .5f, 1, 0, 20);
        }

        mixer.SetFloat("mastervol", f);
    }

    public void ChangeDifficulty() {
        GameManager.IsEasyMode = !GameManager.IsEasyMode;
        difficultySettingText.text = $"DIFFICULTY: {(GameManager.IsEasyMode ? "EASIER" : "REGULAR")}";
    }

    public void SetAimTowardsMouse() {
        GameManager.AimTowardsMouse = !GameManager.AimTowardsMouse;
        aimTowardsMouseText.text = $"AIM TOWARDS MOUSE? {(GameManager.AimTowardsMouse ? "(ON)" : "(OFF)")}";
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void Resume() {
        Time.timeScale = 1;
        canvas.SetActive(false);
        GameManager.instance.EnableGameplay();
    }

    public void Restart() {
        GameManager.instance.Restart();
        Time.timeScale = 1;
    }
}
