using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_HPBar : UIBase
{
    enum GameObjects
    {
        HPBar,
    }

    Stat _stat;

    public override void Init()
    {
        Bind<GameObject>(typeof(GameObjects));
        _stat = transform.parent.GetComponent<Stat>();
    }

    private void Update()
    {
        Transform parent = transform.parent;
        transform.position = parent.position + Vector3.up * (parent.GetComponent<Collider>().bounds.size.y + 0.3f);
        // ī�޶� ���� �������� ������ �ٸ� �����ϸ� �ȴ�.
        transform.rotation = Camera.main.transform.rotation;
        //transform.LookAt(Camera.main.transform);

        float ratio = (float)_stat.Hp / (float)_stat.MaxHp; // MaxHp�� 0�� ��쵵 ó�����־�� ��.  
        SetHPRatio(ratio);

    } 

    public void SetHPRatio(float ratio)
    {
        GameObject go = GetObject((int)GameObjects.HPBar);
        if (go == null)
            return;
        go.GetComponent<Slider>().value = ratio;
    }

}
