using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {
    public GameObject gameCanvas;
    public GameObject hudCanvas;
    public GameObject mainMenuCanvas;

    public GameObject volume;
    public DepthOfField dof;

    public GameObject mainMenuCam;

    public Camera cam;
    public Material pixelMat;

    private void Awake() {
        VolumeProfile volumeProfile = volume.GetComponent<Volume>()?.profile;
        if(!volumeProfile) throw new System.NullReferenceException(nameof(VolumeProfile));

        if(!volumeProfile.TryGet(out dof)) throw new System.NullReferenceException(nameof(dof));
    }

    private void Start() {
        Reset();
    }

    private void Reset() {
        GameManager.instance.EnableUI();
        pixelMat.SetFloat("_mindistance", 7);
        dof.active = true;
    }
    
    public void Play() {
        dof.active = false;
        pixelMat.DOFloat(150, "_mindistance", 3.0f).SetEase(Ease.InQuart);
        GameManager.instance.EnableGameplay();
        
        mainMenuCanvas.SetActive(false);
        hudCanvas.SetActive(true);
        gameCanvas.SetActive(true);
        
        mainMenuCam.SetActive(false);
    }
}
