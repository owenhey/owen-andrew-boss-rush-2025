using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class IntroCutscene : MonoBehaviour {
    public Movement player;
    public Transform loveInterest;
    
    public GameObject playerCamStart;
    public GameObject loveInterestCam;
    public GameObject playerWalksUpCam;
    public GameObject mainMenuCam;
    public GameObject closeUpCam;

    public Transform playerWalkUpPosition;
    public Transform playerCloseWalkUpPosition;

    public MainMenu mm;
    
    [Range(.1f, 10.0f)]
    public float timescale = 1;
        
    public InputActionReference skipAction;
    public InputActionReference actuallySkipAction;

    public GameObject skipCanvas;
    private float TimeSkipped = 100000;

    public HowToPlay howToPlay;
    
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
        playerCamStart.SetActive(false);
        loveInterestCam.SetActive(false);
        playerWalksUpCam.SetActive(false);
        mainMenuCam.SetActive(false);
        closeUpCam.SetActive(false);
        gameObject.SetActive(false);
        mm.GoToGame();
        
        howToPlay.gameObject.SetActive(true);
        howToPlay.ShowHowToPlay();
        
        player.MoveToLocation(mm.gameLoc.position, 1.5f);
    }

    private void Update() {
        Time.timeScale = timescale;
    }

    private IEnumerator PlayRoutine() {
        playerCamStart.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        TextPopups.Instance.Get().PopupAbove("If only I had someone to share this lonely world with...", player.transform, 4.5f).ShowDark().MakePopout();
        yield return new WaitForSeconds(5.0f);
        
        playerCamStart.gameObject.SetActive(false);
        loveInterestCam.gameObject.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        TextPopups.Instance.Get().PopupAbove("Ugh! I've been here for hours!", loveInterest, 2.0f).ShowDark().MakePopout();
        yield return new WaitForSeconds(2.5f);
        TextPopups.Instance.Get().PopupAbove("Usually it doesn't take this long...", loveInterest, 2.0f).ShowDark().MakePopout();
        yield return new WaitForSeconds(2.0f);
        
        loveInterestCam.gameObject.SetActive(false);
        playerWalksUpCam.gameObject.SetActive(true);
            
        player.enabled = true;
        player.MoveToLocation(playerWalkUpPosition.position, 1.5f);
        yield return new WaitForSeconds(1.5f);
        TextPopups.Instance.Get().PopupAbove("Oh wait! Who's that!?", loveInterest, 2.0f).ShowDark();
        yield return new WaitForSeconds(2.0f);
        TextPopups.Instance.Get().PopupAbove("Hey cutie!", loveInterest, 2.0f).ShowDark();
        yield return new WaitForSeconds(3.0f);
        TextPopups.Instance.Get().PopupAbove("Dummy!", loveInterest, 1.5f).ShowDark();
        yield return new WaitForSeconds(1.75f);
        TextPopups.Instance.Get().PopupAbove("Yoohoo! Look over here.", loveInterest, 2.5f).ShowDark();
        yield return new WaitForSeconds(1.0f);
        TextPopups.Instance.Get().PopupAbove("!!!", player.transform, .6f).ShowDark().SetColor(Color.red);
        yield return new WaitForSeconds(.75f);
        
        player.MoveToLocation(playerCloseWalkUpPosition.position, .75f);
        playerWalksUpCam.gameObject.SetActive(false);
        closeUpCam.SetActive(true);
        
        yield return new WaitForSeconds(1.0f);
        TextPopups.Instance.Get().PopupAbove("Aw, is that rose for me??", player.transform).ShowDark();;
        yield return new WaitForSeconds(2.9f);
        TextPopups.Instance.Get().PopupAbove("No.", loveInterest, 1.0f).ShowDark();
        yield return new WaitForSeconds(1.5f);
        TextPopups.Instance.Get().PopupAbove("Not yet, at least.", loveInterest, 2.0f).ShowDark();
        yield return new WaitForSeconds(2.25f);
        TextPopups.Instance.Get().PopupAbove("Pleeeas-", player.transform, 1.0f).ShowDark();
        yield return new WaitForSeconds(.75f);
        TextPopups.Instance.Get().PopupAbove("Shush.", loveInterest, 1.5f).ShowDark();
        yield return new WaitForSeconds(1.5f);
        TextPopups.Instance.Get().PopupAbove("...", player.transform, 2.0f).ShowDark();
        yield return new WaitForSeconds(2.25f);
        TextPopups.Instance.Get().PopupAbove("I don't like begging.", loveInterest, 2.5f).ShowDark();
        yield return new WaitForSeconds(3.5f);
        TextPopups.Instance.Get().PopupAbove("Sorry. What can I do to win that rose and your heart?", player.transform, 4.0f).ShowDark();
        yield return new WaitForSeconds(4.5f);
        TextPopups.Instance.Get().PopupAbove("Hm...", loveInterest, 1.0f).ShowDark();
        yield return new WaitForSeconds(1.25f);
        TextPopups.Instance.Get().PopupAbove("I'll tell you what", loveInterest, 2.0f).ShowDark();
        yield return new WaitForSeconds(2.5f);
        TextPopups.Instance.Get().PopupAbove("This place is infested with evil creatures.", loveInterest, 2.5f).ShowDark();
        yield return new WaitForSeconds(2.5f);
        TextPopups.Instance.Get().PopupAbove("If you're able to, \"take them out\", per se,", loveInterest, 3.0f).ShowDark();
        yield return new WaitForSeconds(3.5f);
        TextPopups.Instance.Get().PopupAbove("Maybe I'll do the same to you.", loveInterest, 3.0f).ShowDark();
        yield return new WaitForSeconds(3.25f);
        TextPopups.Instance.Get().PopupAbove(";)", loveInterest, 1.0f).ShowDark();
        yield return new WaitForSeconds(2.00f);
        TextPopups.Instance.Get().PopupAbove("Say no more! I'm on it!", player.transform, 2.0f).ShowDark();

        yield return new WaitForSeconds(1.0f);
        closeUpCam.SetActive(false);
        skipCanvas.gameObject.SetActive(false);
        
        howToPlay.gameObject.SetActive(true);
        howToPlay.ShowHowToPlay();

        mm.GoToGame();
    }
}
