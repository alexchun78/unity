using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    private void Awake() // 생명주기가 Start보다 빠르다.
    {
        Init();
    }
    void Start()
    {
    
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Game;

        // UI
        Managers.UIManager.ShowSceneUI<UI_Inven>();
    }


    public override void Clear()
    {
    }
}
