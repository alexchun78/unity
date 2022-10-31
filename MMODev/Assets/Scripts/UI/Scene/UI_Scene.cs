using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UIBase
{
    public virtual void Init()
    {
        Managers.UIManager.SetCanvas(gameObject, false);
    }
}
