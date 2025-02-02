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
        movement.enabled = false;
        StartCoroutine(ResetC());
    }

    private IEnumerator ResetC() {
        yield return null;

        mainMenuCam.gameObject.SetActive(true);
        
        GameManager.instance.EnableUI();
        GameManager.instance.EnableCutscene();
        GameManager.instance.EnableGameplay();
        GameManager.instance.EnableRock();
        
        bool killedAllBosses = GameManager.BlobDefeated && GameManager.RobotDefeated && GameManager.SpiderDefeated;
        killedAllBosses = true;
        if (WentPastCutscene && !killedAllBosses) {
            GameManager.instance.EnableUI();
            GameManager.instance.EnableCutscene();
            GameManager.instance.EnableGameplay();
            
            movement.playerCC.enabled = false;
            movement.transform.SetPositionAndRotation(gameLoc.position, gameLoc.rotation);
            movement.targetPositionTrans.position = gameLoc.position;
            movement.playerCC.enabled = true;
            
            pixelMat.SetFloat("_mindistance", 150);
            GameManager.instance.EnableGameplay();
            
            volume.profile = gameProfile;
            
            mainMenuCanvas.SetActive(false);
            hudCanvas.SetActive(true);
            gameCanvas.SetActive(true);
            movement.enabled = true;
            mainMenuCam.SetActive(false);
        }
        else {
            pixelMat.SetFloat("_mindistance", 150);
            movement.playerCC.enabled = false;
            movement.transform.SetPositionAndRotation(mainMenuLoc.position, mainMenuLoc.rotation);
            movement.targetPositionTrans.position = mainMenuLoc.position;
            movement.playerCC.enabled = true;
            
            if (killedAllBosses) {
                fcutscene.gameObject.SetActive(true);
                fcutscene.Play();
                GameManager.instance.EnableCutscene();
                volume.profile = gameProfile;
        
                mainMenuCam.gameObject.SetActive(false);
        
                mainMenuCanvas.SetActive(false);
            }
            else {
                GameManager.instance.EnableGameplay();
                GameManager.instance.EnableUI();
                
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
        movement.enabled = true;
        GameManager.instance.EnableGameplay();
        hudCanvas.SetActive(true);
        gameCanvas.SetActive(true);
    }
}