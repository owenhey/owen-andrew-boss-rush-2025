using System;
using UnityEngine;

public class FlailMovement : MonoBehaviour {

    public float flailSpeed;
    
    public Transform flail;

    public Transform playerParent;

    private Vector3 previousRotation;

    private float totalT = 0;

    private void Start() {
        previousRotation = playerParent.eulerAngles;
    }

    // Update is called once per frame
    void Update() {
        float diff = GetYAngleDifference(previousRotation.y, playerParent.eulerAngles.y);
        previousRotation = playerParent.eulerAngles;

        diff = Mathf.Clamp(diff, -5, 5);
        diff += 5;
        diff /= 10;

        diff = 1 - diff;

        diff *= 2;

        diff = Mathf.Clamp(diff, .3f, 1.7f);
        
        totalT += flailSpeed * diff * Time.deltaTime;
        
        flail.localEulerAngles = new Vector3(0, totalT, 0);
    }
    
    float GetYAngleDifference(float angle1, float angle2)
    {
        // Normalize both angles to be between -180 and 180 degrees
        angle1 = NormalizeAngle(angle1);
        angle2 = NormalizeAngle(angle2);
    
        // Calculate the raw difference
        float difference = angle2 - angle1;
    
        // Handle wraparound by taking the shortest path
        if (difference > 180)
            difference -= 360;
        else if (difference < -180)
            difference += 360;
        
        return difference;
    }

    float NormalizeAngle(float angle)
    {
        // Convert angle to be between -180 and 180
        angle = angle % 360;
        if (angle > 180)
            angle -= 360;
        else if (angle < -180)
            angle += 360;
        return angle;
    }
}
