using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class BagEquip : MonoBehaviour
{
    private Button bagEquipButton;
    private string equipName;
    private string heroName;

    private Action updateHeroMsg;

    /// <summary>
    /// 背包装备初始化
    /// </summary>
    public void BagEquipInit(string heroName, Action updateHeroMsg)
    {
        this.heroName = heroName;
        this.updateHeroMsg = updateHeroMsg;

        bagEquipButton = GetComponent<Button>();
        bagEquipButton.onClick.AddListener(OnBagEquipButtonClick);
        equipName = GetComponent<Image>().sprite.name;
    }

    private void OnBagEquipButtonClick()
    {
        //Debug.Log("出售");
        EquipShopFrame.GetInstance().SellEquip(equipName, heroName);
        if (updateHeroMsg != null)
        {
            updateHeroMsg();
        }
    }
}
