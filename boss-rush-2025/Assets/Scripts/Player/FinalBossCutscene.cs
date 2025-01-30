using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class FinalBossCutscene : MonoBehaviour {
    public Movement player;
    public Transform loveInterest;

    private float TimeSkipped = 100000;

    public Transform playerPos;
    public Transform playerRunaway;
    
    public InputActionReference skipAction;
    public InputActionReference actuallySkipAction;

    public GameObject skipCanvas;

    public GameObject closeUpCam;
    public MainMenu mm;

    public GameObject removeWhenPlay;
    public GameObject enableWhenPlay;

    public Enemy neutralLoveInterest;
    public Enemy realLoveInterest;
    
    public void Play() {
        StartCoroutine(PlayRoutine());
    }

    private void OnEnable() {
        actuallySkipAction.action.performed += Skip;
        skipAction.action.performed += OpenSkip;
    }

    private void OnDisable() {
        actuallySkipAction.action.performed -= Skip;
        skipAction.action.performed -= OpenSkip;
    }
    
    private void OpenSkip(InputAction.CallbackContext obj) {
        skipCanvas.SetActive(true);
        if(TimeSkipped > 10000)
            TimeSkipped = Time.time;
    }

    public void Skip(InputAction.CallbackContext obj) {
        if (Time.time < TimeSkipped + .1f) return;
        
        StopAllCoroutines();
        PlayGame();
    }

    private IEnumerator PlayRoutine() {
        yield return new WaitForSeconds(1.0f);
        closeUpCam.gameObject.SetActive(true);
        player.MoveToLocation(playerPos.position, 2.5f);
        yield return new WaitForSeconds(2.25f);
        
        TextPopups.Instance.Get().PopupAbove("So, I'll be taking that rose now.", player.transform, 3.0f).ShowDark();
        yield return new WaitForSeconds(3.5f);
        
        TextPopups.Instance.Get().PopupAbove("And that date you promised.", player.transform, 3.0f).ShowDark();
        yield return new WaitForSeconds(3.5f);
        
        TextPopups.Instance.Get().PopupAbove("Really?", loveInterest.transform, 1.25f).ShowDark();
        yield return new WaitForSeconds(1.25f);
        
        TextPopups.Instance.Get().PopupAbove("Hm?", player.transform, 1.0f).ShowDark();
        yield return new WaitForSeconds(1.5f);
        
        TextPopups.Instance.Get().PopupAbove("After killing all those kind creatures", loveInterest.transform, 3f).ShowDark();
        yield return new WaitForSeconds(3.25f);
        
        TextPopups.Instance.Get().PopupAbove("You still can't figure it out?", loveInterest.transform, 3f).ShowDark();
        yield return new WaitForSeconds(3.25f);
        
        TextPopups.Instance.Get().PopupAbove("What do you mean!?", player.transform, 2.5f).ShowDark();
        yield return new WaitForSeconds(3f);
        
        TextPopups.Instance.Get().PopupAbove("You fell for the oldest trick in the book!", loveInterest.transform, 3.5f).ShowDark();
        yield return new WaitForSeconds(4f);
        
        TextPopups.Instance.Get().PopupAbove("Now I have free reign!", loveInterest.transform, 3.5f).ShowDark();
        yield return new WaitForSeconds(4f);
        
        TextPopups.Instance.Get().PopupAbove("And can take my rightful place as ruler.", loveInterest.transform, 3.5f).ShowDark();
        yield return new WaitForSeconds(4f);
        
        TextPopups.Instance.Get().PopupAbove("There's just one left to get rid of...", loveInterest.transform, 3.5f).ShowDark();
        yield return new WaitForSeconds(4f);
        
        TextPopups.Instance.Get().PopupAbove("Eek!", player.transform, 1f).ShowDark();
        player.MoveToLocation(playerRunaway.position, .5f);
        yield return new WaitForSeconds(.5f);
        
        PlayGame();
    }

    private void PlayGame() {
        removeWhenPlay.gameObject.SetActive(false);
        enableWhenPlay.gameObject.SetActive(true);
        closeUpCam.SetActive(false);
        skipCanvas.gameObject.SetActive(false);
        mm.GoToGame();

        neutralLoveInterest.gameObject.SetActive(false);
        realLoveInterest.gameObject.SetActive(true);
        realLoveInterest.ShowHealthBar();
    }
}
