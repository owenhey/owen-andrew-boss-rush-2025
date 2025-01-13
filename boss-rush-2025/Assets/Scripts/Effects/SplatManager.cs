using System.Collections.Generic;
using UnityEngine;

public class SplatManager : MonoBehaviour {
    public static SplatManager Instance;

    [SerializeField] private List<Splat> Splats = new();
    [SerializeField] private Splat Prefab;

    public void Awake() {
        Instance = this;
    }        
        
    public Splat Get() {
        for (int i = 0; i < Splats.Count; i++) {
            if (Splats[i].gameObject.activeInHierarchy == false) {
                return Splats[i];
            }
        }

        var newSplat = Instantiate(Prefab, transform);
        Splats.Add(newSplat);
        return newSplat;
    }
}
