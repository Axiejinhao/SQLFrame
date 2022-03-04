using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;

public class ShopView : MonoBehaviour
{
    #region UIComponent
    //商店装备预设体
    private GameObject shopEquipPrefab;
    //背包装备预设体
    private GameObject bagEquipPrefab;
    //商店窗口
    private Transform shopWindow;
    //背包窗口
    private Transform bagWindow;
    //英雄名称
    private Text heroNameText;
    //属性
    private Text[] propertiesText;
    //金钱
    private Text heroMoneyText;
    #endregion

    private EquipShopFrame eFrame;
    private string heroName;

    private void Awake()
    {
        eFrame = EquipShopFrame.GetInstance();
        UIComponentInit();
    }

    private void Start()
    {
        eFrame.OpenDatabase("ShopDatabase");
        //设置英雄名称
        SetHeroNameText();
        //商店初始化
        ShopInit();
        //更新英雄信息
        UpdateHeroMsg();
    }

    /// <summary>
    /// UI组件初始化
    /// </summary>
    private void UIComponentInit()
    {
        shopWindow = transform.Find("ShopWindow").transform;
        bagWindow = transform.Find("BagWindow").transform;
        shopEquipPrefab = Resources.Load<GameObject>("Prefabs/ShopEquip");
        bagEquipPrefab = Resources.Load<GameObject>("Prefabs/BagEquip");
        heroNameText = transform.Find("HeroName").GetComponent<Text>();
        //找到四个属性框的父对象
        Transform propertiesParent = transform.Find("HeroPropertiesText").transform;
        propertiesText = new Text[4];
        //遍历获取文本组件
        for (int i = 0; i < propertiesText.Length; i++)
        {
            propertiesText[i] = propertiesParent.GetChild(i).GetComponent<Text>();
        }

        heroMoneyText = transform.Find("MoneyIcon/Text").GetComponent<Text>();
    }

    /// <summary>
    /// 商店初始化
    /// </summary>
    private void ShopInit()
    {
        string[] equips = eFrame.GetShopEquips();
        //遍历生成所有装备
        for (int i = 0; i < equips.Length; i++)
        {
            //生成装备对象
            GameObject crtEquip = Instantiate(shopEquipPrefab);
            //存放装备图片的格子
            Transform box = shopWindow.GetChild(i);
            //设置父对象
            crtEquip.transform.SetParent(box);
            //设置本地坐标
            crtEquip.transform.localPosition = Vector3.zero;
            //设置本地缩放
            crtEquip.transform.localScale = Vector3.one;
            //获取装备图片[Sprite]
            Sprite equipSprite = Resources.Load<Sprite>("Textures/" + equips[i]);
            //更改装备图片
            crtEquip.GetComponent<Image>().sprite = equipSprite;
            //给装备按钮添加点击事件
            crtEquip.GetComponent<ShopEquip>().ShopEquipInit(heroName, UpdateHeroMsg);
        }
    }

    /// <summary>
    /// 更新英雄信息
    /// </summary>
    private void UpdateHeroMsg()
    {
        SetHeroMoney();
        SetHeroProperties();
        ClearHeroBagEquips();
        SetHeroBagEquips();
    }

    /// <summary>
    /// 设置英雄名称文本
    /// </summary>
    private void SetHeroNameText()
    {
        //获取名称
        heroName = eFrame.GetHeroName();
        //设置名称
        heroNameText.text = heroName;
    }

    /// <summary>
    /// 更新英雄属性
    /// </summary>
    private void SetHeroProperties()
    {
        int[] properties = eFrame.GetHeroProperties(heroName);
        //设置UI
        for (int i = 0; i < propertiesText.Length; i++)
        {
            propertiesText[i].text = properties[i].ToString();
        }
    }

    /// <summary>
    /// 设置英雄的金钱
    /// </summary>
    private void SetHeroMoney()
    {
        int money = eFrame.GetHeroMoney(heroName);
        //设置UI
        heroMoneyText.text = money.ToString();
    }

    /// <summary>
    /// 清空英雄背包装备
    /// </summary>
    private void ClearHeroBagEquips()
    {
        for (int i = 0; i < bagWindow.childCount; i++)
        {
            Transform bagBox = bagWindow.GetChild(i);
            if (bagBox.childCount != 0 && bagBox.name != "Header")
            {
                //将背包格子中的装备销毁
                Destroy(bagBox.GetChild(0).gameObject);
            }
        }
    }

    /// <summary>
    /// 设置英雄背包图片
    /// </summary>
    private void SetHeroBagEquips()
    {
        string[] equips = eFrame.GetHeroEquipsArray(heroName);
        //遍历装备名称
        for (int i = 0; i < equips.Length; i++)
        {
            if (equips[i] == "")
            {
                continue;
            }
            GameObject crtEquip = Instantiate(bagEquipPrefab);
            Transform bagBox = bagWindow.GetChild(i);
            crtEquip.transform.SetParent(bagBox);
            crtEquip.transform.localPosition = Vector3.zero;
            crtEquip.transform.localScale = Vector3.one;
            Sprite equipSprite = Resources.Load<Sprite>("Textures/" + equips[i]);
            crtEquip.GetComponent<Image>().sprite = equipSprite;
            crtEquip.GetComponent<BagEquip>().BagEquipInit(heroName, UpdateHeroMsg);
        }
    }

    private void OnApplicationQuit()
    {
        eFrame.CloseDatabase();
    }
}
