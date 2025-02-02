using System.Collections;
using UnityEngine;

public class HowToPlay : MonoBehaviour {
    public Movement player;
    public PlayerAttacks playerAttacks;
    
    public void ShowHowToPlay() {
        StartCoroutine(ShowHowToPlayC());
    }

    private void Start() {
        lastMove = player.transform.position;
        player.OnRoll += HandleRoll;
        playerAttacks.OnAttack += HandleAttack;
    }
    
    private void HandleRoll() {
        rollCount++;
    }
    
    private void HandleAttack(int attack) {
        if (attack is 1 or 2) {
            attackCount++;
        }

        if (attack is 3) {
            thirdAttackCount++;
        }
    }
    

    private Vector3 lastMove;
    private float totalDistance = 0.0f;
    
    private int rollCount = 0;
    private int attackCount = 0;
    private int thirdAttackCount = 0;
    
    private void Update() {
        if (player.enabled) {
            Vector3 d = player.transform.position - lastMove;
            lastMove = player.transform.position;
            
            totalDistance += d.magnitude;
            
            Debug.Log(totalDistance);
        }
    }

    private IEnumerator ShowHowToPlayC() {
        yield return new WaitForSeconds(2.0f);
        totalDistance = 0;
        string moveText = DetectInputMode.IsController
            ? "Use the left stick to move around."
            : "Use WASD to move around.";

        var textPopup = TextPopups.Instance.Get().PopupAbove(moveText, transform, 1000.0f);
        yield return new WaitUntil(MovedFarEnough);
        
        textPopup.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(.5f);
        string rollText = DetectInputMode.IsController
            ? "Button South to roll"
            : "Space to roll";
        textPopup = TextPopups.Instance.Get().PopupAbove(rollText, transform, 1000.0f);
        rollCount = 0;
        
        yield return new WaitUntil(HaveRolled);
        textPopup.gameObject.SetActive(false);
        
        textPopup = TextPopups.Instance.Get().PopupAbove("Rolling makes you briefly invulernable", transform, 1000.0f);
        rollCount = 0;
        
        yield return new WaitUntil(HaveRolled1);
        
        textPopup.gameObject.SetActive(false);
        
        yield return new WaitForSeconds(.5f);
        string attackText = DetectInputMode.IsController
            ? "Button East to attack"
            : "Left click to attack";
        textPopup = TextPopups.Instance.Get().PopupAbove(attackText, transform, 1000.0f);
        attackCount = 0;
        
        yield return new WaitUntil(HasAttacked);
        textPopup.gameObject.SetActive(false);
        thirdAttackCount = 0;
        yield return new WaitForSeconds(.5f);
        attackText = "Attacking three times allows for a <i>spin</i>";
        textPopup = TextPopups.Instance.Get().PopupAbove(attackText, transform, 1000.0f);
        thirdAttackCount = 0;
        
        yield return new WaitUntil(HasAttackedThree);
        yield return new WaitForSeconds(.5f);
        textPopup.gameObject.SetActive(false);
        gameObject.SetActive(false);
        
        TextPopups.Instance.Get().PopupAbove("Good luck!", transform, 2.0f);
    }

    private bool MovedFarEnough() {
        Debug.Log(totalDistance);
        return totalDistance > 15.0f;
    }
    
    private bool HaveRolled() {
        return rollCount > 1;
    }
    
    private bool HaveRolled1() {
        return rollCount > 0;
    }

    private bool HasAttacked() {
        return attackCount > 1;
    }

    private bool HasAttackedThree() {
        return thirdAttackCount > 0;
    }
}
