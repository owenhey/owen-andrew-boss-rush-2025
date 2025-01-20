using UnityEngine;

public class EnableTerrain : MonoBehaviour {
    public GameObject otherTerrain;

    public TerrainCollider baseTerrain;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        otherTerrain.SetActive(true);
        Destroy(baseTerrain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
