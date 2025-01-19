using System.Collections.Generic;
using UnityEngine;

public class Spider : Enemy {
    public List<SpiderLeg> leftLegs;
    public List<SpiderLeg> rightLegs;

    [Range(0, 1)]
    public float switchLegTime = .25f;
    [Range(0, 1)]
    public float legMoveDistance = .25f;
    [Range(0, 1)]
    public float legMoveTime = .15f;

    public Transform Body;
    
    public Vector3 velocity;
    private Vector3 previousPosition;

    private void Start() {
        for (int i = 0; i < leftLegs.Count; i++) {
            leftLegs[i].spider = rightLegs[i].spider = this;
        }
    }

    protected override void OnUpdate(){
        base.OnUpdate();
        velocity = (transform.position - previousPosition) / Time.deltaTime;
        previousPosition = transform.position;

        LeanTowards(targetPosition);

        Vector3 lookTarget = targetPosition;
        lookTarget.y = transform.position.y;
        LookAt(lookTarget);
        
        
        int count = (int)(Time.time / switchLegTime);
        for (int i = 0; i < leftLegs.Count; i++) {
            bool even = (count + i) % 2 == 0;
            leftLegs[i].CanMove = even;
            rightLegs[i].CanMove = !even;
            
            leftLegs[i].moveThreshold = legMoveDistance;
            rightLegs[i].moveThreshold = legMoveDistance;
            
            leftLegs[i].legMoveTime = legMoveTime;
            rightLegs[i].legMoveTime = legMoveTime;
        }
    }
    
    private void LookAt(Vector3 target) {
        Vector3 lookTowards = target - transform.position;
        lookTowards.y = 0;
        if (lookTowards == Vector3.zero) return;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        cc.transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 6);
    }
    
    private void LeanTowards(Vector3 targetPos) {
        Vector3 towards = targetPos - transform.position;
        towards = Vector3.ClampMagnitude(towards, 1);
        towards.y = 0;
        float magnitude = towards.magnitude;

        Vector3 worldLean = Vector3.up + (towards * (magnitude * .07f));
        worldLean.Normalize();

        // Convert the world-space up vector to local space
        Vector3 localUp = Body.InverseTransformDirection(Vector3.up);
        
        // Convert the target world direction to local space
        Vector3 localTargetDir = Body.InverseTransformDirection(worldLean);
        
        // Calculate the rotation from localUp to localTargetDir
        Quaternion localRotation = Quaternion.FromToRotation(localUp, localTargetDir);

        Body.localRotation = localRotation;
    }
}
