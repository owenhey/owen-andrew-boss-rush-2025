using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour {
    [SerializeField] private TextMeshPro Text;
    [SerializeField] private GameObject darkBG;
    private Transform cam;

    private int textCount = 0;

    public float revealSpeed = .01f;

    private string toldText;
    private void Awake() {
        cam = Camera.main.transform;
    }
    
    private void LateUpdate() {
        Vector3 atCamera = (cam.position - transform.position).normalized;
        Vector3 awayFromCamSpot = transform.position - atCamera;
        transform.LookAt(awayFromCamSpot);
    }
    
    public TextPopup Popup(string text, Vector3 location, float duration = 3.0f, float delay = 0) {
        SetColor(Color.white);
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        toldText = text;
        darkBG.gameObject.SetActive(false);
        textCount = text.Length;
        Text.text = text;
        Text.fontSize = 4.2f;
        transform.position = location + new Vector3(Random.Range(-.25f, .25f), Random.Range(-.25f, .25f), Random.Range(-.25f, .25f));
        transform.DOScale(Vector3.one, .15f).SetDelay(.1f + delay).From(Vector3.zero).OnComplete(() => {
            transform.DOScale(Vector3.zero, .15f).SetDelay(duration).OnComplete(() => {
                gameObject.SetActive(false);
                animating = false;
            });
        });
        return this;
    }

    private IEnumerator ResizeCoroutine() {
        yield return null;
        var textSize = Text.rectTransform.sizeDelta;
        
        float XSize = Text.rectTransform.sizeDelta.x * 10;
        
        if (textCount < 14) {
            XSize = textCount * 3;
        }

        darkBG.transform.localScale = new Vector3(XSize, Text.rectTransform.sizeDelta.y * 10, 1);
    }

    private bool animating = false;
    int index = 0;
    private float previousTime = 0;
    private void Update() {
        if(animating)
        if (Time.time > previousTime + .03f) {
            if (index <= toldText.Length) {
                Text.text = toldText.Insert(index, "<alpha=#00>") + "</alpha>";
                index++;
                previousTime = Time.time;
            }
        }
    }


    public TextPopup SetColor(Color c) {
        Text.color = c;
        return this;
    }
    
    public TextPopup SetSize(float size) {
        Text.fontSize = 4.2f * size;
        return this;
    }
    
    public TextPopup ShowDark() {
        darkBG.gameObject.SetActive(true);
        StartCoroutine(ResizeCoroutine());
        return this;
    }
    
    public TextPopup MakePopout() {
        animating = true;
        index = 0;
        return this;
    }

    public TextPopup Popup(string text, Transform location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location.position, duration, delay);
    }
    
    public TextPopup PopupAbove(string text, Vector3 location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location + Vector3.up * 3.0f, duration, delay);
    }

    public TextPopup PopupAbove(string text, Transform location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location.position + Vector3.up * 3.0f, duration, delay);
    }
}
