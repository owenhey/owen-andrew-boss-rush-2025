using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class RockBehavior : MonoBehaviour {
    public InputActionReference rockSpin;
    public InputActionReference rockThrow;
    private bool movingRock;

    private Transform player;

    private Vector3 vectorAwayFromPlayer;

    private float rotationAmount;

    public Enemy loveInterest;
    
    public IEnumerator Start() {
        player = FindAnyObjectByType<Movement>(FindObjectsInactive.Include).transform;
        yield return null;
        
        GameManager.instance.EnableUI();
        GameManager.instance.EnableCutscene();
        GameManager.instance.EnableGameplay();
        GameManager.instance.EnableRock();
        
        GameManager.instance.EnableGameplay();
    }
    public void Go() {
        GameManager.instance.EnableRock();
        transform.DOShakePosition(.25f, .25f, 30).OnComplete(() => {
            Vector3 endPos = player.position + Vector3.up + Vector3.right * 2;
            transform.DOMove(endPos, .35f).SetDelay(.25f).OnComplete(() => {
                movingRock = true;
                player.DOLookAt(loveInterest.transform.position, .25f);
            });
        });

        player.DOLookAt(loveInterest.transform.position, .5f).OnComplete(() => {
            player.GetComponent<Movement>().ResetTarget();
        });

        vectorAwayFromPlayer = Vector3.right;
    }
    
    Vector2 previousInput = Vector2.up;

    private void OnEnable() {
        rockThrow.action.started += ThrowRock;
    }

    private void OnDisable() {
        rockThrow.action.started -= ThrowRock;
    }

    private void Update() {
        if (!movingRock) return;
        
        Quaternion quat = Quaternion.AngleAxis(rotationAmount * Time.deltaTime, Vector3.up);
        vectorAwayFromPlayer = quat * vectorAwayFromPlayer;
        
        Vector3 endPos = player.position + Vector3.up + vectorAwayFromPlayer * 2;
        transform.position = endPos;
    }

    private void FixedUpdate() {
        if (!movingRock) return;
        Vector2 inputV2 = rockSpin.action.ReadValue<Vector2>();
        
        float angle = Vector2.SignedAngle(previousInput, inputV2);
        previousInput = inputV2;

        if (Mathf.Abs(angle) > 80) return;
        
        float keyboardFactor = .2f;
        float maxRotation = 75;
        rotationAmount += Mathf.Clamp(angle * keyboardFactor , -maxRotation, maxRotation);
    }

    private void ThrowRock(InputAction.CallbackContext obj) {
        if (!movingRock) return;
        movingRock = false;

        Vector3 cross = Vector3.Cross(Vector3.up, vectorAwayFromPlayer);
        cross.Normalize();
        float force = rotationAmount / 50.0f;
        
        Debug.Log("force: " + force);

        Vector3 final = transform.position + cross * force;
        final.y = 0;

        transform.DOJump(final, 1, 1, .5f).SetEase(Ease.Linear);
        
        GameManager.instance.EnableGameplay();
        Destroy(this);
    }
}
