using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Inven : UI_Scene
{
    enum GameObjects
    {
        GridPanel,
    }

    public override void Init()
    {
        base.Init();

        Bind<GameObject>(typeof(GameObjects));

        GameObject gridPanel = Get<GameObject>((int)GameObjects.GridPanel);
        foreach(Transform child in gridPanel.transform)
        {
            Managers.Resource.Destory(child.gameObject);
        }

        // 실제 인벤토리 정보를 참고해서 만들 것 
        for (int i = 0; i < 8; ++i)
        {
            UI_Inven_item invenItem = Managers.UIManager.MakeSubItem<UI_Inven_item>(gridPanel.transform);
            invenItem.SetInfo($"검 {i}번");
            //GameObject item = Managers.Resource.Instantiate("UI/Scene/UI_Inven_Item");
            // item.transform.SetParent(gridPanel.transform);
            //UI_Inven_item invenItem = item.GetOrAddComponent<UI_Inven_item>();
            //invenItem.SetInfo($"검 {i}번");
        }
    }
}
