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
        //DetectObject();
    }

    // private void DetectObject()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
    //     RaycastHit2D[] allHits = Physics2D.GetRayIntersectionAll(ray);
    //     for(int i = 0; i < allHits.Length; i++)
    //     {
    //         if(allHits[0].collider != null)
    //         {
    //             Debug.Log(allHits[0].collider.name);
    //         }
    //     }
        
    // }
}