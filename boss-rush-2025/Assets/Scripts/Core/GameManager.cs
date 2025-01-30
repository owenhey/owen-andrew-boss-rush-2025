using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public AudioMixerGroup mixer;

    public PlayerInput input;

    public static bool SpiderDefeated;
    public static bool RobotDefeated;
    public static bool BlobDefeated;

    public static string lastBossKilled;

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

    public void EnableCutscene() {
        Debug.Log("Enabling: Cutscene");
        input.SwitchCurrentActionMap("Cutscene");
    }

    public void Restart() {
        SceneManager.LoadScene(0);
    }

    public void Reset() {
        EnableUI();

        var allEnemies = GameObject.FindObjectsByType<Enemy>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        foreach (var enemy in allEnemies) {
            enemy.StopAllCoroutines();
            enemy.InCombat = false;
            enemy.SayPlayerDeathText();
        }
        
        StartCoroutine(delayRestart());

        IEnumerator delayRestart() {
            yield return new WaitForSeconds(4.0f);
            SceneManager.LoadScene(0);
        }
    }
}
