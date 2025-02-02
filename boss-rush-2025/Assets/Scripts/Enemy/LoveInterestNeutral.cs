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
    
    public void RespondToRespawn() {
        if (GameManager.lastBossFought == "The Blob") {
            Debug.Log("Responding to blob");
            if (GameManager.killedBoss) {
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove("Thanks for taking care of that gross blob for me!", transform, 3.0f, 1.5f);
            }
            else {
                string[] texts = {
                    "What happened down there?",
                    "Not the most impressive showing...",
                    "Why'd I even ask you to do this?",
                    "Did the little guys get you?"
                };
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove(texts[Random.Range(0, texts.Length)], transform, 3.0f, 1.5f);
            }
        }
        if (GameManager.lastBossFought == "Spider") {
            Debug.Log("Responding to spider");
            if (GameManager.killedBoss) {
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove("Thanks! That thing gave me the creeps", transform, 3.0f, 1.5f);
            }
            else {
                string[] texts = {
                    "How'd you end up dying to that thing?",
                    "I'm unimpressed...",
                    "Really? A spider got you?",
                    "I was hoping for better..."
                };
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove(texts[Random.Range(0, texts.Length)], transform, 3.0f, 1.5f);
            }
        }
        if (GameManager.lastBossFought == "Robot") {
            Debug.Log("Responding to robot");
            if (GameManager.killedBoss) {
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove("Nice job taking care of that thing up there!", transform, 3.0f, 1.5f);
            }
            else {
                string[] texts = {
                    "What happened up there?",
                    "Did the lasers get you?",
                    "Let me guess, you rolled into a laser.",
                    "Next time, try *avoiding* the lasers"
                };
                Sound.I.PlayLoveInterestVoice(1.5f);
                TextPopups.Instance.Get()
                    .PopupAbove(texts[Random.Range(0, texts.Length)], transform, 3.0f, 1.5f);
            }
        }
    }
}
