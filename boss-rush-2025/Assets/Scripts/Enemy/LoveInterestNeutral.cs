using UnityEngine;

public class LoveInterestNeutral : Enemy {
    public Transform head;

    private Vector3 headTargetPos;

    public Transform playerLookAtCenter;

    protected override void OnUpdate() {
        base.OnUpdate();

        if ((player.transform.position - playerLookAtCenter.position).magnitude < 3) {
            headTargetPos = player.transform.position + Vector3.up * 1.5f;
        }
        else {
            headTargetPos = transform.position + transform.forward + Vector3.up * 1.5f;
            
        }
        
        Vector3 lookTowards = headTargetPos - transform.position;
        lookTowards.y = 0;
            
        Quaternion targetRotation = Quaternion.LookRotation(lookTowards);
        head.rotation = Quaternion.Slerp(head.rotation, targetRotation, Time.deltaTime * 3);
        
        if((int)(Time.time * 10) % 100 == 0) {
            InCombat = false;
        }
    }
}
