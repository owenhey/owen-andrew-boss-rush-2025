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
        GameManager.instance.EnableGameplay();
        GameManager.instance.EnableUI();
    }
    
    public void Play() {
        Debug.Log("playing!");
        pixelMat.SetFloat("_mindistance", 150);
        GameManager.instance.EnableGameplay();
        
        volume.profile = gameProfile;

        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        gameCanvas.SetActive(true);
        
        mainMenuCam.SetActive(false);
    }
}