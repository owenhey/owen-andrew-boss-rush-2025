using UnityEngine;

public class RotateNearby : MonoBehaviour {
    public Movement player;

    private float t;
    private void Update() {
        if (!player) return;

        float dis = (transform.position - player.transform.position).magnitude;
        dis = Mathf.Clamp(dis, 3, 10);
        t += Time.deltaTime * ((11 - dis) * (11 - dis)) * 15;

        transform.localEulerAngles = new Vector3(0, t, 0);
    }
}
