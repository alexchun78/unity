using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIButton : MonoBehaviour
{
    [SerializeField]
    Text _text;

    int score = 0;
    public void OnButtonCliecked()
    {
        Debug.Log("Button Clicked");
        score++;
        _text.text = $"Á¡¼ö : {score}";
    }
}
