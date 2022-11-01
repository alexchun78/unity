using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoginScene : BaseScene
{
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // Build Setting에서 먼저 Scene을 등록해주어야 한다.
            Managers.SceneEX.LoadScene(Define.Scene.Game);
        }
    }

    protected override void Init()
    {
        base.Init();

        SceneType = Define.Scene.Login;
    }

    public override void Clear()
    {
        Debug.Log("LoginScene Clear!");
    }
}
