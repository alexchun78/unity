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

        Get<Text>((int)Texts.ScoreText).text = "Bind Text";

        GameObject go = GetImage((int)Images.ItemIcon).gameObject;
        UI_EventHandler evt = go.GetComponent<UI_EventHandler>();
        evt.OnDragHandler += ((PointerEventData data) => { evt.gameObject.transform.position = data.position; });
    }


    //int score = 0;
    //public void OnButtonCliecked()
    //{
    //    Debug.Log("Button Clicked");
    //    score++;
    //    _text.text = $"Á¡¼ö : {score}";
    //}
}
