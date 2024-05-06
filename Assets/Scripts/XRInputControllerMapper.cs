using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class XRInputControllerMapper : MonoBehaviour
{
    [Header("Primary Button (Left)")]
    [SerializeField] private InputActionReference inputAction_PrimaryButtonLeft;
    [SerializeField] private UnityEvent onPrimaryButtonLeftPressed;
    
    [Header("Secondary Button (Left)")]
    [SerializeField] private InputActionReference inputAction_SecondaryButtonLeft;
    [SerializeField] private UnityEvent onSecondaryButtonLeftPressed;
    
    [Header("Primary Button (Right)")]
    [SerializeField] private InputActionReference inputAction_PrimaryButtonRight;
    [SerializeField] private UnityEvent onPrimaryButtonRightPressed;
    
    [Header("Secondary Button (Right)")]
    [SerializeField] private InputActionReference inputAction_SecondaryButtonRight;
    [SerializeField] private UnityEvent onSecondaryButtonRightPressed;
    
    void Awake()
    {
        inputAction_PrimaryButtonLeft.action.started += _ =>
        {
            onPrimaryButtonLeftPressed?.Invoke();
        };
        
        inputAction_SecondaryButtonLeft.action.started += _ =>
        {
            onSecondaryButtonLeftPressed?.Invoke();
        };
        
        inputAction_PrimaryButtonRight.action.started += _ =>
        {
            onPrimaryButtonRightPressed?.Invoke();
        };
        
        inputAction_SecondaryButtonRight.action.started += _ =>
        {
            onSecondaryButtonRightPressed?.Invoke();
        };
    }
}
