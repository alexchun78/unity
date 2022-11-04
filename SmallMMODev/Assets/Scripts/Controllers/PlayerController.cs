using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    PlayerStat _stat;
    Vector3 _destPoint = new Vector3(0.0f, 0.0f, 0.0f);

    GameObject _lockTarget;
    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);

    public enum PlayerState
    {
        Die, 
        Moving,
        Idle,
        Skill,
    }
    [SerializeField]
    PlayerState _state = PlayerState.Idle;
    public PlayerState State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch(_state)
            {
                case PlayerState.Idle:
                    anim.SetFloat("speed", 0);
                    anim.SetBool("attack", false);
                    break;
                case PlayerState.Moving:
                    {
                        anim.SetFloat("speed", _stat.MoveSpeed);
                        anim.SetBool("attack", false);
                    }
                    break;
                case PlayerState.Skill:
                    anim.SetBool("attack", true);
                    break;
                case PlayerState.Die:
                    anim.SetBool("attack", false);
                    break;
            }
        }
    }

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
            _destPoint = _lockTarget.transform.position;
            var dist = (_destPoint - transform.position).magnitude;
            if (dist <= 1) //1.2
            {
                State = PlayerState.Skill;
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
                    State = PlayerState.Idle;
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
            State = PlayerState.Idle;
        }              
    }

    private void UpdateIdle()
    {
        State = PlayerState.Idle;
    }

    private void UpdateSkill()
    {
      //  Debug.Log("UpdateSkill()");
        //if (_bStopSkill)
        //{
        //    State = PlayerState.Idle;
        //}
        //else
        //{
        //    State = PlayerState.Skill;
        //}
      //  State = PlayerState.Moving;
    }

    void OnHitEvent()
    {
        Debug.Log("OnHitEvent_TEST");
        if (_bStopSkill)
        {
            State = PlayerState.Idle;
        }
        else
        {
            State = PlayerState.Skill;
        }
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

    bool _bStopSkill = false;
    void OnMouseEvent(Define.MouseEvent evt)
    {
       // Debug.Log(_state);
        switch (State)
        {
            case PlayerState.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case PlayerState.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _bStopSkill = true;
                }
                break;
            case PlayerState.Die:
                break;
        }
    }

    void OnMouseEvent_IdleRun(Define.MouseEvent evt)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        bool bRayHit = Physics.Raycast(ray, out hit, 100.0f, _mask);
        Debug.DrawRay(ray.origin, ray.direction * 100.0f, Color.blue, 1.0f);

        switch (evt)
        {
            case Define.MouseEvent.PointerDown: //Input.GetMouseButtonDown(0)
                {
                    if (bRayHit == true)
                    {
                        _destPoint = hit.point;
                        State = PlayerState.Moving;
                        _bStopSkill = false;

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
                    if(_lockTarget == null && bRayHit == true)
                        _destPoint = hit.point;
                }
                break;
            case Define.MouseEvent.PointerUp:
                _bStopSkill = true;
                break;
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
