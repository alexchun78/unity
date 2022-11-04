using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : UI_Popup
{
    enum Buttons
    {
        PointButton,
    }

    enum Texts
    {
        PointText,
        ScoreText,
    }

    enum GameObjects
    {
        TestObject, 
    }

    enum Images
    {
        ItemIcon,
    }

    public override void Init()
    {
        base.Init();

        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        // extention�� �����Ͽ� ����ϸ�, �Ʒ�ó�� �ڵ带 ������ ���� �� �ִ�. 
        GetButton((int)Buttons.PointButton).gameObject.BindEvent(OnButtonCliecked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        BindEvent(go, ((PointerEventData data) => { go.transform.position = data.position; }), Define.UIEvent.Drag);
    }


    int score = 0;
    public void OnButtonCliecked(PointerEventData data)
    {
        Debug.Log("Button Clicked");
        score++;

        Get<Text>((int)Texts.ScoreText).text = $"���� : {score}";
    }
}
