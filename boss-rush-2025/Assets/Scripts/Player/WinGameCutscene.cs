using System;
using System.Collections;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class WinGameCutscene : MonoBehaviour {
    public Movement player;

    public GameObject closeUpCam;

    public Transform endPos1;
    public Transform endPos2;

    public GameObject endCanvas;
    public CanvasGroup canvasGroup;

    public Button btn;

    public void Play() {
        StartCoroutine(PlayRoutine());
    }

    private IEnumerator PlayRoutine() {
        Music.I.PlayHub();
        yield return new WaitForSeconds(1.0f);
        closeUpCam.gameObject.SetActive(true);
        player.MoveToLocation(endPos1.position, 1.5f);
        yield return new WaitForSeconds(1.55f);
        player.MoveToLocation(endPos2.position, .25f);
        TextPopups.Instance.Get().PopupAbove("       ...", player.transform.position, 1.0f).MakePopout().ShowDark();
        yield return new WaitForSeconds(1.5f);
        TextPopups.Instance.Get().PopupAbove("Yet another lesson in love's futility.<br>", player.transform.position, 3.0f).MakePopout().ShowDark();
        yield return new WaitForSeconds(3.5f);
        TextPopups.Instance.Get().PopupAbove("At least I ended up with the rose!<br>", player.transform.position).MakePopout().ShowDark();
        yield return new WaitForSeconds(3.5f);
        TextPopups.Instance.Get().PopupAbove("Thank you for playing!", player.transform.position).MakePopout().ShowDark();
        yield return new WaitForSeconds(2.0f);
        EndGame();
    }

    private void EndGame() {
        endCanvas.gameObject.SetActive(true);
        canvasGroup.DOFade(1, 2).From(0);
        GameManager.instance.EnableUI();
        btn.Select();
    }

    public void Quit() {
        Application.Quit();
    }
}
