using System;
using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;

public class CaveEntrance : MonoBehaviour {
    public Movement movement;

    public Transform landingPosition;
    public float time;
    public float height;

    public CinemachineCamera cam;
    public CinemachineCamera blobCam;
    
    public void OnTriggerEnter(Collider other) {
        if (other.TryGetComponent(out Movement m)) {
            movement.Cutscened = true;
            cam.gameObject.SetActive(true);
            movement.transform.DOJump(landingPosition.position, height, 1, time).SetEase(Ease.Linear).OnComplete(() => {
                movement.Cutscened = false;
                cam.gameObject.SetActive(false);
                blobCam.gameObject.SetActive(true);
            });
        }
    }
}
