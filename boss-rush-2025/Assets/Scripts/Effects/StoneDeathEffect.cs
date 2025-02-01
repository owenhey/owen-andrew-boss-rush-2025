using System.Collections.Generic;
using UnityEngine;

public class StoneDeathEffect : MonoBehaviour {
    public List<MeshRenderer> toUpdate1;
    public List<MeshRenderer> toUpdate2;
    public List<SkinnedMeshRenderer> toUpdate3;
    
    public Material mat1update;
    public Material mat2update;
    public Material mat3update;

    public bool IsSpider;
    public bool IsRobot;
    public bool IsBlob;
    
    private void Start() {
        if (IsSpider && GameManager.SpiderDefeated) {
            foreach (var mr in toUpdate1) {
                mr.material = mat1update;
            }
            foreach (var mr in toUpdate2) {
                mr.material = mat2update;
            }
        }
        if (IsRobot && GameManager.RobotDefeated) {
            foreach (var mr in toUpdate1) {
                mr.material = mat1update;
            }
            foreach (var mr in toUpdate2) {
                mr.material = mat2update;
            }
        }
        if (IsBlob && GameManager.BlobDefeated) {
            foreach (var mr in toUpdate1) {
                mr.material = mat1update;
            }
            foreach (var mr in toUpdate2) {
                mr.material = mat2update;
            }
            foreach (var mr in toUpdate3) {
                mr.material = mat2update;
            }
        }
    }
}
