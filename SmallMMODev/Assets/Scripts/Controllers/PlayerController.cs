using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : BaseController
{
    PlayerStat _stat;

    int _mask = (1 << (int)Define.Layer.Ground) | (1 << (int)Define.Layer.Monster);
    bool _bStopSkill = false;

    public override void Init()
    {
#if false
        Managers.Input.KeyAction -= OnKeyBoard;
        Managers.Input.KeyAction += OnKeyBoard;
#endif
        WorldObjectType = Define.WorldObject.Player;
        _stat = gameObject.GetComponent<PlayerStat>();

        Managers.Input.MouseAction -= OnMouseEvent;
        Managers.Input.MouseAction += OnMouseEvent;

        // UI
        // Managers.UIManager.ShowSceneUI<UI_Inven>();

        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        {
            Managers.UIManager.MakeWorldSpaceUI<UI_HPBar>(transform);
        }
    }

    protected override void UpdateMoving()
    {
        // 몬스터를 찾았을 경우에만 작동되게
        // 몬스터가 사정거리 내이면 공격상태로 변경
        if(_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            var dist = (_destPos - transform.position).magnitude;
            Debug.Log($"dist: {dist}");
            if (dist <= 1.7) //1.2
            {
                State = Define.State.Skill;
                return;
            }
        }

        // Mouse 이동
        Vector3 dir = _destPos - transform.position;
        dir.y = 0;
        if (dir.magnitude > 0.1)
        {
#if false     
            bool bHit = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized, 1.0f, LayerMask.GetMask("Block"));
            if(bHit == true)
            {
                if (!Input.GetMouseButton(0))
                    State = Define.State.Idle;
                return;
            }

            // navs Calculate Path
            float moveDist = Math.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);

            NavMeshAgent navs = gameObject.GetComponent<NavMeshAgent>();
            navs.Move(dir.normalized * moveDist);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime * _stat.MoveSpeed * 2);


#else
            bool bHit = Physics.Raycast(transform.position + Vector3.up * 0.5f, dir.normalized, 1.0f, (1 << (int)Define.Layer.Block));
            if(bHit == true)
            {
                Debug.Log("Block");
                if (!Input.GetMouseButton(0))
                    State = Define.State.Idle;
                return;
            }

            float moveDist = Math.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);
            transform.position += (dir.normalized * moveDist);
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir.normalized), Time.deltaTime * _stat.MoveSpeed * 2);
#endif
        }
        else
        {
            State = Define.State.Idle;
        }              
    }

    protected override void UpdateSkill()
    {
        if(_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime *20);
        }
    }
#if false
        if (_bMoveToDest == true)
        {
            // Mouse 이동
            Vector3 dir = _destPos - transform.position;
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
    void OnHitEvent()
    {
        Debug.Log("OnHitEvent_TEST");

        if (_lockTarget != null)
        {
            Stat targetStat = _lockTarget.GetComponent<Stat>();
           // PlayerStat myStat = gameObject.GetComponent<PlayerStat>();

            targetStat.OnAttacked(_stat); 
            //int damage = Mathf.Max(myStat.Attack - targetStat.Defense, 0);
            //Debug.Log(damage);

            //targetStat.Hp -= damage;
        }

        if (_bStopSkill)
        {
            State = Define.State.Idle;
        }
        else
        {
            State = Define.State.Skill;
        }
    }

    void OnMouseEvent(Define.MouseEvent evt)
    {
       // Debug.Log(_state);
        switch (State)
        {
            case Define.State.Idle:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Moving:
                OnMouseEvent_IdleRun(evt);
                break;
            case Define.State.Skill:
                {
                    if (evt == Define.MouseEvent.PointerUp)
                        _bStopSkill = true;
                }
                break;
            case Define.State.Die:
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
                        _destPos = hit.point;
                        State = Define.State.Moving;
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
                        _destPos = hit.point;
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
