using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager
{
    public Action KeyAction = null;
    public Action<Define.MouseEvent> MouseAction = null;

    bool _pressed = false;
    float _pressedTime = 0.0f;

    public void OnUpdate()
    {
        // UI 클릭 제외 : 
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.anyKey == true && KeyAction != null)
            KeyAction.Invoke();

        if(MouseAction != null)
        {
            if(Input.GetMouseButton(0)) // 마우스를 눌렀을 때, 
            {
                if(_pressed == false)
                {
                    MouseAction.Invoke(Define.MouseEvent.PointerDown);
                    _pressedTime = Time.time;
                }
                MouseAction.Invoke(Define.MouseEvent.Press);
                _pressed = true;
            }
            else
            {
                if(_pressed == true) // 마우스를 클릭했을 때, (마우스 누른 상태에서 누른 손가락을 놨을 때)
                {
                    if (Time.time < _pressedTime + 0.2f)
                    {
                        MouseAction.Invoke(Define.MouseEvent.Click);
                    }
                    MouseAction.Invoke(Define.MouseEvent.PointerUp);
                }
                _pressedTime = 0.0f;
                _pressed = false;
            }
        }
    }

    public void Clear()
    {
       KeyAction = null;
       MouseAction = null;
    }
}

