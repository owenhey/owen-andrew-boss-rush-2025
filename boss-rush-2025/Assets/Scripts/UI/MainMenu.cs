using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject gameCanvas;
    public GameObject hudCanvas;
    public GameObject mainMenuCanvas;

    public Volume volume;
    public VolumeProfile mainProfile;
    public VolumeProfile gameProfile;

    public GameObject mainMenuCam;

    public Camera cam;
    public Material pixelMat;

    public Transform mainMenuLoc;
    public Transform gameLoc;

    public Movement movement;

    public IntroCutscene cutscene;
    public FinalBossCutscene fcutscene;

    public static bool WentPastCutscene;
    
    private void Start() {
        Reset();
    }

    private void Reset() {
        pixelMat.SetFloat("_mindistance", 7);

        volume.profile = mainProfile;

        StartCoroutine(ResetC());
    }

    private IEnumerator ResetC() {
        yield return null;
        
        mainMenuCam.gameObject.SetActive(true);
        
        GameManager.instance.EnableUI();
        GameManager.instance.EnableCutscene();
        GameManager.instance.EnableGameplay();
        GameManager.instance.EnableRock();
        
        Debug.Log("Main 1");
        bool killedAllBosses = GameManager.BlobDefeated && GameManager.RobotDefeated && GameManager.SpiderDefeated;
        if (WentPastCutscene && !killedAllBosses) {
            GameManager.instance.EnableUI();
            GameManager.instance.EnableCutscene();
            GameManager.instance.EnableGameplay();
            movement.transform.SetPositionAndRotation(gameLoc.position, gameLoc.rotation);
            
            pixelMat.SetFloat("_mindistance", 150);
            GameManager.instance.EnableGameplay();
            
            volume.profile = gameProfile;
            Debug.Log("Main 2");
            
            mainMenuCanvas.SetActive(false);
            hudCanvas.SetActive(true);
            gameCanvas.SetActive(true);
            
            mainMenuCam.SetActive(false);
        }
        else {
            Debug.Log("Main 3");
            if (killedAllBosses) {
                Debug.Log("killed all bosses");
                fcutscene.gameObject.SetActive(true);
                fcutscene.Play();
                Debug.Log("Main 4");
                GameManager.instance.EnableCutscene();
                volume.profile = gameProfile;
        
                mainMenuCam.gameObject.SetActive(false);
        
                mainMenuCanvas.SetActive(false);
            }
            else {
                GameManager.instance.EnableGameplay();
                GameManager.instance.EnableUI();
                movement.transform.SetPositionAndRotation(mainMenuLoc.position, mainMenuLoc.rotation);
                Debug.Log("Main 5");
        
                mainMenuCanvas.SetActive(true);
                hudCanvas.SetActive(false);
                gameCanvas.SetActive(false);
        
                mainMenuCam.SetActive(true);
            }
        }
    }
    
    public void Play() {
        Debug.Log("playing!");
        pixelMat.SetFloat("_mindistance", 150);
        GameManager.instance.EnableCutscene();
        volume.profile = gameProfile;
        
        mainMenuCam.gameObject.SetActive(false);

        mainMenuCanvas.SetActive(false);
        
        cutscene.gameObject.SetActive(true);
        cutscene.Play();
        WentPastCutscene = true;
    }

    public void GoToGame() {
        GameManager.instance.EnableGameplay();
        hudCanvas.SetActive(true);
        gameCanvas.SetActive(true);
    }
}