using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAnimationController : MonoBehaviour
{
    private Animator _animator => GetComponent<Animator>();

    private bool _isActive = false;
    private static readonly int IsActive = Animator.StringToHash("IsActive");

    public bool GetState()
    {
        return _isActive;
    }
    public void ToggleState()
    {
        _isActive = !_isActive; 
        _animator.SetBool(IsActive, _isActive);
    }

    public void SetActiveState()
    {
        _isActive = true;
        _animator.SetBool(IsActive, _isActive);
    }
    
    public void SetUnactiveState()
    {
        _isActive = false;
        _animator.SetBool(IsActive, _isActive);
    }

}
