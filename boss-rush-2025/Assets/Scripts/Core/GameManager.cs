using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    public InputActionAsset actions;

    private void Awake() {
        instance = this;

        PlayerHealth.OnDie += Reset;
    }

    private void OnDestroy() {
        PlayerHealth.OnDie -= Reset;
    }

    public void EnableGameplay() {
        Debug.Log("Enabling: Gameplay");
        actions.FindActionMap("Player").Enable();
        actions.FindActionMap("UI").Disable();
    }

    public void EnableUI() {
        Debug.Log("Enabling: UI");
        actions.FindActionMap("Player").Disable();
        actions.FindActionMap("UI").Enable();
    }
    
    public void Reset() {
        StartCoroutine(delayRestart());

        IEnumerator delayRestart() {
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(0);
        }
    }
}
