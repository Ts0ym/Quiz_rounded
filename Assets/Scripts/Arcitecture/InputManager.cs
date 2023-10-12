using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput _playerInput => GetComponent<PlayerInput>();

    private InputAction _touchPositionAction;
    private InputAction _isTouchedAction;

    private bool _isDragging = false;

    public InputAction TouchPosition => _touchPositionAction;
    public InputAction IsTouched => _isTouchedAction;

    private void Awake()
    {
        _touchPositionAction = _playerInput.actions.FindAction("TouchPosition");
        _isTouchedAction = _playerInput.actions.FindAction("TouchContact");
    }

    private void OnEnable()
    {
        _isTouchedAction.started += OnSwipeStart;
        _isTouchedAction.canceled += OnSwipeEnds;
    }

    private void OnSwipeStart(InputAction.CallbackContext ctx)
    {
        _isDragging = true;
    }

    private void OnSwipeEnds(InputAction.CallbackContext ctx)
    {
        _isDragging = false;
    }
}
