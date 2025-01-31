using System.Collections;
using DG.Tweening;
using UnityEngine;

public class SpinnerEnemy : Enemy {
    public LayerMask lm;
    public DamageInstance di;
    private void Start() {
        Vector2 onUnit = Random.insideUnitCircle.normalized;
        
        // Raycast at collider layer
        Ray ray = new Ray(transform.position, new Vector3(onUnit.x, 0, onUnit.y));
        if (Physics.Raycast(ray, out RaycastHit hit, 50, lm)) {

            targetPosition = hit.point;
        }

        StartCoroutine(ResetDamage());

        transform.DOScale(Vector3.one, .35f).SetEase(Ease.OutElastic).From(0);
        ShouldMove = false;
        cc.enabled = false;
        transform.DOJump(transform.position, 2, 1, .35f).SetEase(Ease.Linear).OnComplete(() => {
            ShouldMove = true;
            cc.enabled = true;
        });
    }

    private IEnumerator ResetDamage() {
        while (true) {
            yield return new WaitForSeconds(.5f);
            di.Reset();
        }
    }

    protected override void OnUpdate() {
        if ((transform.position - targetPosition).magnitude < 2) {
            Vector2 onUnit = Random.insideUnitCircle.normalized;
        
            // Raycast at collider layer
            Ray ray = new Ray(transform.position, new Vector3(onUnit.x, 0, onUnit.y));
            if (Physics.Raycast(ray, out RaycastHit hit, 50, lm)) {
                targetPosition = hit.point;
            }
        }
    }
}
