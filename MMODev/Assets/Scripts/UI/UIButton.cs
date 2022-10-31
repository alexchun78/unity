using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIButton : UIBase
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

    private void Start()
    {
        Bind<Button>(typeof(Buttons));
        Bind<Text>(typeof(Texts));
        Bind<GameObject>(typeof(GameObjects));
        Bind<Image>(typeof(Images));


        // extention을 선언하여 사용하면, 아래처럼 코드를 나누지 않을 수 있다. 
        GetButton((int)Buttons.PointButton).gameObject.AddUIEvent(OnButtonCliecked);

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        AddUIEvent(go, ((PointerEventData data) => { go.transform.position = data.position; }), Define.UIEvent.Drag);
    }

    int score = 0;
    public void OnButtonCliecked(PointerEventData data)
    {
        Debug.Log("Button Clicked");
        score++;

        Get<Text>((int)Texts.ScoreText).text = $"점수 : {score}";
    }
}
