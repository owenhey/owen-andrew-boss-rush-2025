using DG.Tweening;
using TMPro;
using UnityEngine;

public class TextPopup : MonoBehaviour {
    [SerializeField] private TextMeshPro Text;

    public void Popup(string text, Vector3 location, float duration = 3.0f) {
        gameObject.SetActive(true);
        Text.text = text;
        transform.position = location;
        transform.DOScale(Vector3.one, .15f).From(Vector3.zero).OnComplete(() => {
            transform.DOScale(Vector3.zero, .15f).SetDelay(duration).OnComplete(() => {
                gameObject.SetActive(false);
            });
        });
    }

    public void Popup(string text, Transform location, float duration = 3.0f) {
        Popup(text, location.position, duration);
    }
    
    public void PopupAbove(string text, Vector3 location, float duration = 3.0f) {
        Popup(text, location + Vector3.up * 2.5f, duration);
    }

    public void PopupAbove(string text, Transform location, float duration = 3.0f) {
        Popup(text, location.position + Vector3.up * 2.5f, duration);
    }
}
