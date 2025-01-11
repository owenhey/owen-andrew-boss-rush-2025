using System.Collections.Generic;
using UnityEngine;

public class TextPopups : MonoBehaviour {
    public static TextPopups Instance;

    [SerializeField] private List<TextPopup> Popups = new();
    [SerializeField] private TextPopup Prefab;

    public void Awake() {
        Instance = this;
    }        
        
    public TextPopup Get() {
        for (int i = 0; i < Popups.Count; i++) {
            if (Popups[i].gameObject.activeInHierarchy == false) {
                return Popups[i];
            }
        }

        var newPopup = Instantiate(Prefab, transform);
        Popups.Add(newPopup);
        return newPopup;
    }
}
