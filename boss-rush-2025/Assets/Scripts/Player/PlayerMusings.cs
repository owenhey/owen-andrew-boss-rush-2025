using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerMusings : MonoBehaviour {
    
    [TextArea(3,3)]
    public List<string> strings;
    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer is 7 or 8) {
            var randomSTring = strings[Random.Range(0, strings.Count)];
            TextPopups.Instance.Get().PopupAbove(randomSTring, Movement.GetPlayerPos(), 2.5f).MakePopout();
            Destroy(gameObject);
        }
    }
}
