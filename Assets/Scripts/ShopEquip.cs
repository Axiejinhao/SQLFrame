using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class ShopEquip : MonoBehaviour
{
    private Button shopEquipButton;
    private string equipName;
    private string heroName;
    //更新英雄信息事件
    private Action updateHeroMsg;

    /// <summary>
    /// 装备初始化
    /// </summary>
    public void ShopEquipInit(string heroName, Action updateHeroMsg)
    {
        //更新英雄名称
        this.heroName = heroName;
        //更新英雄信息事件
        this.updateHeroMsg = updateHeroMsg;
        shopEquipButton = GetComponent<Button>();
        //绑定点击事件
        shopEquipButton.onClick.AddListener(OnShopEquipButtonClick);
        //获取装备名称
        equipName = GetComponent<Image>().sprite.name;
    }

    public void OnShopEquipButtonClick()
    {
        //Debug.Log("购买");
        //进行数据库操作
        EquipShopFrame.GetInstance().BuyEquip(equipName, this.heroName);
        //将数据库数据更新到UI
        if (updateHeroMsg != null)
        {
            this.updateHeroMsg();
        }
    }
}
