using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterController : BaseController
{
    Stat _stat;

    [SerializeField]
    float _scanRange = 10.0f;

    [SerializeField]
    float _attackRange = 2.0f;

    public override void Init()
    {
        WorldObjectType = Define.WorldObject.Monster;

        _stat = gameObject.GetComponent<Stat>();

        if(gameObject.GetComponentInChildren<UI_HPBar>() == null)
        {
            Managers.UIManager.MakeWorldSpaceUI<UI_HPBar>(transform);
        }
    }

    protected override void UpdateIdle()
    {
        Debug.Log("Monster Update Idle");

        //// TODO : 매니저가 생기면 변경 가능
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
            return;

        float distance = (player.transform.position - transform.position).magnitude;
        Debug.Log($"Distance of Player btw Monster : {distance}");
        if (distance <= _scanRange)
        {
            _lockTarget = player;
            State = Define.State.Moving;
            return;
        }
    }

    protected override void UpdateSkill() 
    {
        Debug.Log("Monster Update Skill");

        if (_lockTarget != null)
        {
            Vector3 dir = _lockTarget.transform.position - transform.position;
            Quaternion quat = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, quat, Time.deltaTime * 20);
        }
    }
    protected override void UpdateMoving()
    {
        Debug.Log("Monster Update Moving");

        // 플레이어를 찾았을 경우에만 작동되게
        // 플레이어가 사정거리 내이면 공격상태로 변경
        if (_lockTarget != null)
        {
            _destPos = _lockTarget.transform.position;
            var dist = (_destPos - transform.position).magnitude;
            Debug.Log($"dist: {dist}");
            if (dist <= _attackRange) //1.2
            {
                State = Define.State.Skill;
                NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
                nma.SetDestination(transform.position);
                return;
            }
        }

        // Mouse 이동
        Vector3 dir = _destPos - transform.position;
        if (dir.magnitude > 0.1)
        {
            float moveDist = Math.Clamp(Time.deltaTime * _stat.MoveSpeed, 0, dir.magnitude);
            dir = dir.normalized;

            NavMeshAgent nma = gameObject.GetComponent<NavMeshAgent>();
            nma.SetDestination(_destPos);
            nma.speed = _stat.MoveSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime * _stat.MoveSpeed * 2);
        }
        else
        {
            State = Define.State.Idle;
        }
    }

    void OnHitEvent()
    {

        if (_lockTarget == null)
        {
            State = Define.State.Idle;
            return;
        }

        Stat targetStat = _lockTarget.GetComponent<Stat>();
        int damage = Mathf.Max(_stat.Attack - targetStat.Defense, 0);

        targetStat.Hp -= damage;

        // 상대가 죽었는 지 확인
        Debug.Log(targetStat.Hp);
        if (targetStat.Hp > 0)
        {
            float dist = (_lockTarget.transform.position - gameObject.transform.position).magnitude;
            if (dist <= _attackRange) //1.2
            {
                State = Define.State.Skill;
            }
            else
            {
                State = Define.State.Moving;
            }
        }
        else
        {
            Managers.Game.DeSpawn(_lockTarget);
            State = Define.State.Idle;
        }
    }
}
