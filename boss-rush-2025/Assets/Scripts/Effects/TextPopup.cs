using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour {
    [SerializeField] private TextMeshPro Text;

    public TextPopup Popup(string text, Vector3 location, float duration = 3.0f, float delay = 0) {
        SetColor(Color.white);
        gameObject.SetActive(true);
        transform.localScale = Vector3.one;
        Text.text = text;
        Text.fontSize = 4.2f;
        transform.position = location;
        transform.DOScale(Vector3.one, .15f).SetDelay(delay).From(Vector3.zero).OnComplete(() => {
            transform.DOScale(Vector3.zero, .15f).SetDelay(duration).OnComplete(() => {
                gameObject.SetActive(false);
            });
        });
        return this;
    }

    public TextPopup SetColor(Color c) {
        Text.color = c;
        return this;
    }
    
    public TextPopup SetSize(float size) {
        Text.fontSize = 4.2f * size;
        return this;
    }

    public TextPopup Popup(string text, Transform location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location.position, duration, delay);
    }
    
    public TextPopup PopupAbove(string text, Vector3 location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location + Vector3.up * 2.5f, duration, delay);
    }

    public TextPopup PopupAbove(string text, Transform location, float duration = 3.0f, float delay = 0) {
        return Popup(text, location.position + Vector3.up * 2.5f, duration, delay);
    }
}
