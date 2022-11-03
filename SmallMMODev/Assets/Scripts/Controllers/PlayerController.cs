using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destPoint = new Vector3(0.0f, 0.0f, 0.0f);

    public enum PlayerState
    {
        Die, 
        Moving,
        Idle,
        Skill,
    }

    PlayerState _state = PlayerState.Idle;


    void Start()
    {
#if false
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;
#endif
        _stat = gameObject.GetComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        // UI
        Managers.UIManager.ShowSceneUI<UI_Inven>();
    }

    private void UpdateDie()
    {
        // 현재는 아무것도 할 게 없음
    }

    private void UpdateMoving()
    {
        // 몬스터를 찾았을 경우에만 작동되게
        // 몬스터가 사정거리 내이면 공격상태로 변경
        if(_lockTarget != null)
        {
            var dist = (_destPoint - gameObject.transform.position).magnitude;
            //Debug.Log(dist);
            if(dist < 1.2)
            {
                _state = PlayerState.Skill;
                return;
            }
        }

        // Mouse 이동
        Vector3 dir = _destPoint - transform.position;
        if (dir.magnitude > 0.1)
        {
#if true     // navs Calculate Path
            float moveDist = Math.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);
            dir = dir.normalized;

            NavMeshAgent navs = gameObject.GetComponent<NavMeshAgent>();
        
            navs.Move(dir * moveDist);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _stat.MoveSpeed * 2);

            bool bHit = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir, 1.0f, LayerMask.GetMask("Block"));
            if(bHit == true)
            {
                if (!Input.GetMouseButton(0))
                    _state = PlayerState.Idle;
                return;
            }

#else
            float moveDist = Math.Clamp(Time.deltaTime * _speed, 0, dir.magnitude);
            dir = dir.normalized;
            transform.position += (dir * moveDist);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _speed * 2);
#endif
        }
        else
        {
            _state = PlayerState.Idle;
        }

        // animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetFloat("speed", _stat.MoveSpeed);                   
    }

    private void UpdateIdle()
    {
        // animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetFloat("speed", 0);       
    }

    private void UpdateSkill()
    {
        // animation
        Animator anim = GetComponent<Animator>();
        // 현재 게임 상태에 대한 정보를 넘겨준다.
        anim.SetBool("attack", true);
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
            case PlayerState.Skill:
                UpdateSkill();
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

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    GameObject _lockTarget;
    void OnMouseEvent(Define.MouseEvent evt)
    {
        if (_state == PlayerState.Die)
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool bRayHit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        // Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue, 1.0f);

        switch(evt)
        {
            case Define.MouseEvent.PointerDown: //Input.GetMouseButtonDown(0)
                {
                    if(bRayHit == true)
                    {
                        _destPoint = hit.point;
                        _state = PlayerState.Moving;

                        if (hit.collider.gameObject.layer == (int)Define.Layer.Monster)
                        {
                            _lockTarget = hit.collider.gameObject;
                        }
                        else
                        {
                            _lockTarget = null;
                        }
                    }
                }
                break;
            case Define.MouseEvent.Press: // Input.GetMouseButton(0)
                {
                    if(_lockTarget != null)
                    {
                        _destPoint = _lockTarget.transform.position;
                    }
                    else
                    {
                        if (bRayHit)
                            _destPoint = hit.point;
                    }
                }
                break;
            //case Define.MouseEvent.PointerUp: // Input.GetMouseButtonUp(0)
            //    _lockTarget = null;
            //    break;
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
