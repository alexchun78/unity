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
        // UI Ŭ�� ���� : 
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        if (Input.anyKey == true && KeyAction != null)
            KeyAction.Invoke();

        if(MouseAction != null)
        {
            if(Input.GetMouseButton(0)) // ���콺�� ������ ��, 
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
                if(_pressed == true) // ���콺�� Ŭ������ ��, (���콺 ���� ���¿��� ���� �հ����� ���� ��)
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

