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

        if (DeathRoutine.DiedBefore) {
            GameManager.instance.EnableUI();
            GameManager.instance.EnableGameplay();
            movement.transform.SetPositionAndRotation(gameLoc.position, gameLoc.rotation);
            
            pixelMat.SetFloat("_mindistance", 150);
            GameManager.instance.EnableGameplay();
        
            volume.profile = gameProfile;

            mainMenuCanvas.SetActive(false);
            hudCanvas.SetActive(true);
            gameCanvas.SetActive(true);
        
            mainMenuCam.SetActive(false);
        }
        else {
            GameManager.instance.EnableGameplay();
            GameManager.instance.EnableUI();
            movement.transform.SetPositionAndRotation(mainMenuLoc.position, mainMenuLoc.rotation);
            
        
            mainMenuCanvas.SetActive(true);
            hudCanvas.SetActive(false);
            gameCanvas.SetActive(false);
        
            mainMenuCam.SetActive(true);
        }
    }
    
    public void Play() {
        Debug.Log("playing!");
        pixelMat.SetFloat("_mindistance", 150);
        
        volume.profile = gameProfile;

        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        gameCanvas.SetActive(true);
        
        mainMenuCam.SetActive(false);

        movement.MoveToLocation(gameLoc.position, 1.5f);
        Invoke(nameof(EnableGameplay), 1.5f);
    }

    private void EnableGameplay() {
        GameManager.instance.EnableGameplay();
    }
}