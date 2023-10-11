using System;
using UnityEngine;

public class InputManager
{
    public bool IsActive = true;
    public Action moveAction = null;
    public Action jumpAction = null;
    public Action changeAction = null;
    public Action objectAction = null;
    public Action recallAction = null;
    public Action camAction = null;
    public Action recallCheckAction = null;
    public void ForFixedUpdate()
    {
        if (IsActive && (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0))
            moveAction?.Invoke();
    }
    public void ForUpdate()
    {
        if (IsActive)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                changeAction?.Invoke();

            if (Input.GetKeyDown(KeyCode.E))
                objectAction?.Invoke();

            if (Input.GetKeyUp(KeyCode.F))
                recallAction?.Invoke();

            if (Input.GetKey(KeyCode.F))
                recallCheckAction?.Invoke();

            if (Input.GetMouseButton(1))
                camAction?.Invoke();

            if (Input.GetKeyDown(KeyCode.Space))
                jumpAction?.Invoke();
        }
    }
}
