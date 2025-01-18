using DG.Tweening;
using DitzelGames.FastIK;
using UnityEngine;

public class SpiderLeg : MonoBehaviour {
    public FastIKFabric IKSolver;

    public Spider spider;
    public Transform SpiderParent;
    public Transform Body;
    public Transform DesiredTransform;
    private Transform ikTransform;

    private Vector3 localDesiredFootOffset;
    private float spiderYPosition;

    public float moveThreshold = .25f;
    public float legMoveTime = .15f;
    public bool CanMove = true;

    private bool isMoving = false;
    private void Awake() {
        IKSolver.Target.SetParent(null, true);
        DesiredTransform.SetParent(Body, true);

        ikTransform = IKSolver.Target;
        
        localDesiredFootOffset = DesiredTransform.localPosition;
        spiderYPosition = SpiderParent.position.y;
    }

    private void Update() {
        Vector3 desiredFootDir = Body.TransformDirection(localDesiredFootOffset);
        Vector3 desiredFootPos = Body.position + desiredFootDir;
        desiredFootPos.y = spiderYPosition;

        DesiredTransform.position = desiredFootPos;

        float sqThreshold = moveThreshold * moveThreshold;
        
        if (!CanMove) return;
        if (isMoving) return;
        
        if ((ikTransform.position - DesiredTransform.position).sqrMagnitude > sqThreshold) {
            isMoving = true;

            Vector3 desired = DesiredTransform.position;
            desired += spider.velocity * (legMoveTime);
            
            ikTransform.DOJump(desired, .5f, 1, legMoveTime).OnComplete(() => {
                isMoving = false;
            });
        }
    }
}
