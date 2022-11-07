using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Define.CameraMode _cameraMode = Define.CameraMode.QuaterView;

    [SerializeField]
    Vector3 _delta = new Vector3(0.0f, 6.0f, -5.0f);

    [SerializeField]
    GameObject _player = null;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log($"{ _delta.x},{ _delta.y},{ _delta.z}");

        Managers.Input.MouseAction -= MouseEvent;
        Managers.Input.MouseAction += MouseEvent;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(_cameraMode == Define.CameraMode.QuaterView)
        {
            // �÷��̾ �׾��� ��, 
            if(_player == null)
            {


                return;
            }


            // ����ĳ��Ʈ�� �Ѵ�.
            RaycastHit hit;
            bool bRtn = Physics.Raycast(_player.transform.position, _delta, out hit, _delta.magnitude, LayerMask.GetMask("Wall"));
            if(bRtn == true)
            {
                Debug.Log("Wall existed");
                // �浹������, �� ���̷� ī�޶� �̵���Ų��.
                var dist = (hit.point - _player.transform.position).magnitude * 0.8f;
                transform.position = _player.transform.position + _delta.normalized * dist;
            }
            else
            {                
                transform.position = _player.transform.position + _delta;
                transform.LookAt(_player.transform);
            }

        }

    }

    public void SetQuaterView(Vector3 delta)
    {
        _cameraMode = Define.CameraMode.QuaterView;
        _delta = delta;
    }

    void MouseEvent(Define.MouseEvent evt)
    {


        Debug.Log("Camera mouse");
    }
}
