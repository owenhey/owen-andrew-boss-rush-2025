using UnityEngine;

public class TrainingDummy : Enemy {
    public float radiusAllowed = 5;
    private Vector3 startPos;

    private void Awake() {
        startPos = transform.position;
    }
    protected override void OnUpdate() {
        if ((transform.position - startPos).magnitude > radiusAllowed) {
            transform.position = startPos;
        }
    }
}
