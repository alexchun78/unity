using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScene : BaseScene
{
    private void Awake() // �����ֱⰡ Start���� ������.
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
