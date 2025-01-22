using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private void Awake() {
        instance = this;

        PlayerHealth.OnDie += Reset;
    }

    private void OnDestroy() {
        PlayerHealth.OnDie -= Reset;
    }
    
    public void Reset() {
        StartCoroutine(delayRestart());

        IEnumerator delayRestart() {
            yield return new WaitForSeconds(2.0f);
            SceneManager.LoadScene(0);
        }
    }
}
