using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Random = UnityEngine.Random;

public class LittleSpider : MonoBehaviour {
    [System.Serializable]
    public struct SpiderRow {
        public List<Transform> points;
    }
    public List<SpiderRow> points;

    public int row = 3;

    public bool moving = false;

    private Transform currentTarget;
    private int currentCol;

    public List<Transform> feet;

    public float speed = 35;
    public float turingSpeed = 15;

    public DamageInstance damage;

    public int direction = 1;
    
    private void Start() {
        currentCol = Random.Range(0, 5);
        currentTarget = points[row].points[GetIndex(currentCol + direction)];
        
        damage.OnHit += OnHit;
        
        damage.gameObject.SetActive(false);

        transform.LookAt(currentTarget.position);
        transform.DOJump(currentTarget.position, 1.5f, 1, .5f).SetDelay(.2f).SetEase(Ease.InQuad).OnComplete(() => {
            moving = true;
            damage.gameObject.SetActive(true);
        });
    }

    private void OnHit() {
        Invoke(nameof(LetHitAgain), 1.0f);
    }

    private void LetHitAgain() {
        damage.Reset();
    }

    private void Update() {
        if (!moving) return;
        
        Vector3 towardsTarget = currentTarget.position - transform.position;
        if (towardsTarget.magnitude < .1f) {
            currentCol += direction;
            currentTarget = points[row].points[GetIndex(currentCol + direction)];
            towardsTarget = currentTarget.position - transform.position;
        }
        
        Quaternion targetRotation = Quaternion.LookRotation(towardsTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, turingSpeed * Time.deltaTime);
        
        transform.position += towardsTarget.normalized * (speed * Time.deltaTime);

        for (var index = 0; index < feet.Count; index++) {
            var foot = feet[index];
            float t = Time.time * 25.0f;
            Vector3 localRot = new Vector3(0, 0, (index % 2 == 0 ? -1 : 1) * 40 * Mathf.Sin(t));
            foot.localEulerAngles = localRot;
        }
    }

    private int GetIndex(int col) {
        return (col + 100000) % points[0].points.Count;
    }

    private void OnDrawGizmos() {
        foreach (var p in points) {
            foreach (var s in p.points) {
                Gizmos.DrawSphere(s.position, .25f);
            }
        }
    }
}
