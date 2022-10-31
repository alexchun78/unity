using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    float _speed = 10.0f;
    //bool _bMoveToDest = false;
    Vector3 _destPoint = new Vector3(0.0f, 0.0f, 0.0f);
   // float _wait_run_ratio = 0.0f;

    public enum PlayerState
    {
        Die, 
        Moving,
        Idle,
    }

    PlayerState _state = PlayerState.Idle;

    void Start()
    {
#if false
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;
#endif
        Managers.Input.MouseAction -= OnMouseClicked;
        Managers.Input.MouseAction += OnMouseClicked;

    }

    private void UpdateDie()
    {
        // 현재는 아무것도 할 게 없음
    }

    private void UpdateMoving()
    {
        // Mouse 이동
        Vector3 dir = _destPoint - transform.position;
        if (dir.magnitude > 0.0001)
        {
            float moveDist = Math.Clamp(Time.deltaTime * _speed, 0, dir.magnitude);
            dir = dir.normalized;
            transform.position += (dir * moveDist);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _speed * 2);
        }
        else
        {
            _state = PlayerState.Idle;
        }

        // animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetFloat("speed", _speed);

       // _wait_run_ratio = Mathf.Lerp(_wait_run_ratio, 1.0f, _speed * Time.deltaTime);
       // anim.SetFloat("wait_run_ratio", _wait_run_ratio);
        //anim.Play("WAIT_RUN");                      
    }

    private void UpdateIdle()
    {
        // animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetFloat("speed", 0);
        //_wait_run_ratio = Mathf.Lerp(_wait_run_ratio, 0.0f, _speed * Time.deltaTime);
        // anim.SetFloat("wait_run_ratio", _wait_run_ratio);
        //anim.Play("WAIT_RUN");
    }

    void Update()
    {
        switch (_state)
        {
            case PlayerState.Die:
                UpdateDie();
                break;
            case PlayerState.Moving:
                UpdateMoving();
                break;
            case PlayerState.Idle:
            default:
                UpdateIdle();
                break;
        }

#if false
        if (_bMoveToDest == true)
        {
            // Mouse 이동
            Vector3 dir = _destPoint - transform.position;
            if (dir.magnitude > 0.0001)
            {
                float moveDist = Math.Clamp(Time.deltaTime * _speed, 0, dir.magnitude);
                dir = dir.normalized;
                transform.position += (dir * moveDist);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _speed * 2);
            }
            else
            {
                _bMoveToDest = false;
            }
        }

        Animator anim = GetComponent<Animator>();
        if (_bMoveToDest == true)
        {
            _wait_run_ratio = Mathf.Lerp(_wait_run_ratio, 1.0f, _speed * Time.deltaTime);
            anim.SetFloat("wait_run_ratio", _wait_run_ratio);
            anim.Play("WAIT_RUN");
            //anim.SetFloat("wait_run_ratio", 1);
        }
        else
        {
            _wait_run_ratio = Mathf.Lerp(_wait_run_ratio, 0.0f, _speed * Time.deltaTime);
            anim.SetFloat("wait_run_ratio", _wait_run_ratio);
            anim.Play("WAIT_RUN");
            //anim.SetFloat("wait_run_ratio", 0);
        }
#endif
    }

    void OnMouseClicked(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;
#if false
        //if (evt != Define.MouseEvent.Click)
        //    return;
#endif
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue, 1.0f);
        RaycastHit hit;
        bool bRtn = Physics.Raycast(ray, out hit, 100.0f, LayerMask.GetMask("Wall"));
        if (bRtn == true)
        {

            //Debug.Log("OnMouseClicked");
            // _bMoveToDest = true;
            _state = PlayerState.Moving;
            _destPoint = hit.point;
        }
    }

#if false
    // [local -> world]
    // transform.TransformDirection()
    // [world -> local]
    // transform.InverseTransformDirection()
    void OnKeyBoard()
    {
        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.forward), 0.3f);
            // transform.position += new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime * _speed;
            //transform.rotation = Quaternion.LookRotation(Vector3.forward);
            // transform.Translate(Vector3.forward * Time.deltaTime * _speed);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.back), 0.3f);
            //transform.position -= new Vector3(0.0f, 0.0f, 1.0f) * Time.deltaTime * _speed;
            // transform.rotation = Quaternion.LookRotation(Vector3.back);
            //transform.Translate(Vector3.back * Time.deltaTime * _speed);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.left), 0.3f);
            //transform.position -= new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * _speed;
            //transform.rotation = Quaternion.LookRotation(Vector3.left);
            //transform.Translate(Vector3.left * Time.deltaTime * _speed);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.TransformDirection(Vector3.forward * Time.deltaTime * _speed);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(Vector3.right), 0.3f);
            //transform.position += new Vector3(1.0f, 0.0f, 0.0f) * Time.deltaTime * _speed;
            //transform.rotation = Quaternion.LookRotation(Vector3.right);
            //transform.Translate(Vector3.right * Time.deltaTime * _speed);
        }
        _bMoveToDest = false;
    }
#endif
}
