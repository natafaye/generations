using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public event Action<InputValue> OnNavigateInput;
    public event Action<InputValue> OnScrollWheelInput;
    public event Action OnPointInput;
    public event Action<InputValue> OnClickInput;

    private void OnNavigate(InputValue value)
    {
        OnNavigateInput?.Invoke(value);
    }

    private void OnScrollWheel(InputValue value)
    {
        OnScrollWheelInput?.Invoke(value);
    }

    private void OnPoint()
    {
        OnPointInput?.Invoke();
    }

    private void OnClick(InputValue value) 
    {
        OnClickInput?.Invoke(value);
    }
}