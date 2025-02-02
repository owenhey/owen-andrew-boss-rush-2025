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

    public DamageInstance damage;
    
    private float rotationAmount;

    public Enemy loveInterest;
    
    public IEnumerator Start() {
        player = FindAnyObjectByType<Movement>(FindObjectsInactive.Include).transform;
        damage.Enabled = false;
        yield return null;
        
        GameManager.instance.EnableUI();
        GameManager.instance.EnableCutscene();
        GameManager.instance.EnableGameplay();
        GameManager.instance.EnableRock();
        
        GameManager.instance.EnableGameplay();

        damage.OnHit += StopRock;
    }

    private void StopRock() {
        if (transform == null) return;
        
        transform.DOKill();
        transform.DOShakePosition(.4f, 1, 50);
        transform.DOScale(0, .35f).OnComplete(() => {
            Destroy(gameObject);
        });
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
        damage.Enabled = true;

        string text = DetectInputMode.IsController
            ? "Use the left stick to swing the rock around!"
            : "Circle with WASD to swing the rock around!";
        TextPopups.Instance.Get().PopupAbove(text, transform, 1.5f, .5f);
        
        text = DetectInputMode.IsController
            ? "(Button East) to release!"
            : "Space to release!";
        TextPopups.Instance.Get().PopupAbove(text, transform, 2.0f, 2f);
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
        rotationAmount += (angle * keyboardFactor * -1);

        rotationAmount = Mathf.Clamp(rotationAmount, -maxRotSpeed, maxRotSpeed);
    }

    private float maxRotSpeed = 600;

    private void ThrowRock(InputAction.CallbackContext obj) {
        if (!movingRock) return;
        if (Mathf.Abs(rotationAmount) < 10) return;
        
        movingRock = false;
        
        damage.damage = Mathf.Lerp(75, 150, Mathf.Abs(rotationAmount) / maxRotSpeed);

        Vector3 cross = Vector3.Cross(Vector3.up, vectorAwayFromPlayer);
        cross.Normalize();
        float force = (rotationAmount / maxRotSpeed) * 15;
        

        Vector3 towardsenemy = loveInterest.transform.position - transform.position;
        towardsenemy.Normalize();

        float dot = Vector3.Dot(towardsenemy, cross);

        float negative = 1; 

        if (rotationAmount < 0) {
            cross *= -1;
            negative = -1;
            dot *= -1;
        }

        if (dot > .1f) {
            Debug.Log("Good throw!");
            cross = cross * .25f + towardsenemy * .75f;
        }

        Vector3 final = transform.position + cross * force * negative;
        final.y = 0;
        

        transform.DOJump(final, 1, 1, .5f).SetEase(Ease.Linear).OnComplete(() => {
            transform.DOShakePosition(.4f, 1, 50);
            transform.DOScale(0, .35f).OnComplete(() => {
                Destroy(gameObject);
            });
        });
    }

    private void OnDestroy() {
        GameManager.instance.EnableGameplay();
    }
}
