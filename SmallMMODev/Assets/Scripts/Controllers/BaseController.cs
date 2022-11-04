using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseController : MonoBehaviour
{
    #region SerializeField
    [SerializeField]
    protected Vector3 _destPos = new Vector3(0.0f, 0.0f, 0.0f);

    [SerializeField]
    protected GameObject _lockTarget;

    [SerializeField]
    protected Define.State _state = Define.State.Idle;
    public Define.State State
    {
        get { return _state; }
        set
        {
            _state = value;

            Animator anim = GetComponent<Animator>();
            switch (_state)
            {
                case Define.State.Idle:
                    //anim.SetFloat("speed", 0);
                    //anim.SetBool("attack", false);
                    anim.CrossFade("WAIT", 0.1f);
                    break;
                case Define.State.Moving:
                    anim.CrossFade("RUN", 0.1f);
                    //anim.SetFloat("speed", _stat.MoveSpeed);
                    //anim.SetBool("attack", false);
                    break;
                case Define.State.Skill:
                    //anim.CrossFade("ATTACK", 0.1f);
                    anim.CrossFade("ATTACK", 0.1f, -1, 0); // 반복적으로 행동하는 loof 버전
                    //anim.SetBool("attack", true);
                    break;
                case Define.State.Die:
                    //anim.SetBool("attack", false);
                    break;
            }
        }
    }
    #endregion


    private void Start()
    {
        Init();   
    }

    protected virtual void Init() { }

    void Update()
    {
        switch (_state)
        {
            case Define.State.Die:
                UpdateDie();
                break;
            case Define.State.Moving:
                UpdateMoving();
                break;
            case Define.State.Skill:
                UpdateSkill();
                break;
            case Define.State.Idle:
            default:
                UpdateIdle();
                break;
        }
    }

    protected virtual void UpdateDie() { }
    protected virtual void UpdateMoving() { }
    protected virtual void UpdateSkill() { }
    protected virtual void UpdateIdle() { }
}
