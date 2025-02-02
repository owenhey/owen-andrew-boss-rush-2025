using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class DetectInputMode : MonoBehaviour {
    private static DetectInputMode instance;

    [SerializeField] private InputActionReference keyboardmouse;
    [SerializeField] private InputActionReference keyboardmouse2;
    [SerializeField] private InputActionReference controller;
    [SerializeField] private InputActionReference controller2;

    public static bool IsController = false;
    
    public void Awake() {
        instance = this;
        keyboardmouse.action.started += KeyboardOnStarted;
        keyboardmouse2.action.started += KeyboardOnStarted;
        controller.action.started += ControllerOnStarted;
        controller2.action.started += ControllerOnStarted;
    }

    private void OnDestroy() {
        keyboardmouse.action.started -= KeyboardOnStarted;
        keyboardmouse2.action.started -= KeyboardOnStarted;
        controller.action.started -= ControllerOnStarted;
        controller2.action.started -= ControllerOnStarted;
    }

    private void KeyboardOnStarted(InputAction.CallbackContext obj) {
        IsController = false;
    }
    
    private void ControllerOnStarted(InputAction.CallbackContext obj) {
        IsController = true;
    }
}
