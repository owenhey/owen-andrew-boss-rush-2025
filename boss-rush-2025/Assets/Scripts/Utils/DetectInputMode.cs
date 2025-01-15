using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectInputMode : MonoBehaviour {
    private static DetectInputMode instance;

    [SerializeField] private InputActionReference keyboardmouse;
    [SerializeField] private InputActionReference controller;

    public static bool IsController = false;
    
    public void Awake() {
        instance = this;
        keyboardmouse.action.started += KeyboardOnStarted;
        controller.action.started += ControllerOnStarted;
    }

    private void KeyboardOnStarted(InputAction.CallbackContext obj) {
        IsController = false;
    }
    
    private void ControllerOnStarted(InputAction.CallbackContext obj) {
        IsController = true;
    }
}
