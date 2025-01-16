using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    public static GameManager instance;

    private void Awake() {
        instance = this;

        PlayerHealth.OnDie += Reset;
    }
    public void Reset() {
        SceneManager.LoadScene(0);
    }
}
