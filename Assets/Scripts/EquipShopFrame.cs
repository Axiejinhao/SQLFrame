using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EquipShopFrame : SQLFrame
{
    #region Singleton
    private static EquipShopFrame instance;

    public static new EquipShopFrame GetInstance()
    {
        if (instance == null)
        {
            instance = new EquipShopFrame();
        }
        return instance;
    }

    private EquipShopFrame() { }
    #endregion

    private string query;

    public void BuyEquip(string equipName, string heroName)
    {
        #region 购买装备的流程
        //1.查看装备的价钱
        int equipMoney = GetEquipMoney(equipName);
        //2.查看英雄的金钱
        int heroMoney = GetHeroMoney(heroName);
        //3.判断金钱是否足够
        if (equipMoney > heroMoney)
        {
            //4.否,拒绝购买
            Debug.Log("金钱不足");
            return;
        }

        //5.是,购买装备
        //5.1 获取装备属性加成
        int[] equipProperties = GetEquipProperties(equipName);
        //5.2 获取英雄属性
        int[] heroProperties = GetHeroProperties(heroName);
        //5.3 属性相加
        int[] newHeroProperties = PropertiesOperation(heroProperties, equipProperties, 1);
        //5.4 更新属性
        SetHeroProperties(heroName, newHeroProperties);
        //5.5 获取英雄装备存储的字符串
        string heroEquips = GetHeroEquips(heroName);
        //5.6 拼接字符串,形成新的英雄装备信息
        heroEquips += equipName + "|";
        //5.7 将新装备信息存储到数据库
        SetHeroEquips(heroName, heroEquips);
        //5.8 扣除花费,更新英雄的金钱
        SetHeroMoney(heroName, heroMoney - equipMoney);
        #endregion
    }

    public void SellEquip(string equipName, string heroName)
    {
        #region 购买装备的流程
        //1.查看装备的价钱
        int equipMoney = GetEquipMoney(equipName);
        //2.查看英雄的金钱
        int heroMoney = GetHeroMoney(heroName);
        //3. 加钱并更新
        heroMoney += equipMoney / 2;
        SetHeroMoney(heroName, heroMoney);
        //4. 获取装备属性
        int[] equipProperties = GetEquipProperties(equipName);
        //5. 获取英雄属性
        int[] heroProperties = GetHeroProperties(heroName);
        //6. 相减并更新英雄的属性信息
        int[] newHeroProperties = PropertiesOperation(heroProperties, equipProperties, 2);
        SetHeroProperties(heroName, newHeroProperties);
        //7. 获取英雄的装备字符串
        string heroEquips = GetHeroEquips(heroName);
        //8. 获取移除装备的装备字符串下标
        int equipIndex = heroEquips.LastIndexOf(equipName);
        //9. 移除相应装备
        heroEquips = heroEquips.Remove(equipIndex, equipName.Length + 1);
        //10. 更新数据库
        SetHeroEquips(heroName, heroEquips);
        #endregion
    }

    /// <summary>
    /// 属性操作
    /// </summary>
    /// <param name="heroPro"></param>
    /// <param name="equipPro"></param>
    /// <param name="operationID">1 for add, 2 for sub</param>
    /// <returns></returns>
    public int[] PropertiesOperation(int[] heroPro, int[] equipPro, int operationID)
    {
        int[] res = new int[heroPro.Length];
        if (operationID == 1)
        {
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = heroPro[i] + equipPro[i];
            }
        }
        else if (operationID == 2)
        {
            for (int i = 0; i < res.Length; i++)
            {
                res[i] = heroPro[i] - equipPro[i];
            }
        }
        return res;
    }

    /// <summary>
    /// 将新装备信息存储到数据库
    /// </summary>
    /// <param name="heroName"></param>
    /// <param name="heroEquips"></param>
    public void SetHeroEquips(string heroName, string heroEquips)
    {
        query = "update HeroTable set HeroEquips='" + heroEquips + "' where HeroName='" + heroName + "';";
        UpdateExe(query);
    }

    /// <summary>
    /// 获取英雄装备字符串数组
    /// </summary>
    /// <param name="heroName"></param>
    /// <returns></returns>
    public string[] GetHeroEquipsArray(string heroName)
    {
        string equips = GetHeroEquips(heroName);
        //拆分字符串
        return equips.Split('|');
    }

    /// <summary>
    /// 获取英雄装备存储的字符串
    /// </summary>
    /// <param name="heroName"></param>
    /// <returns></returns>
    public string GetHeroEquips(string heroName)
    {
        query = "select HeroEquips from HeroTable where HeroName='" + heroName + "';";
        object res = SelectSingleData(query);
        if (res == null)
        {
            return null;
        }
        return res.ToString();
    }

    /// <summary>
    /// 更新属性
    /// </summary>
    /// <param name="properties"></param>
    public void SetHeroProperties(string heroName, int[] properties)
    {
        query = "update HeroTable set HeroAD=" + properties[0] + ",HeroAP=" + properties[1] + ",HeroAR=" + properties[2] + ",HeroSR=" + properties[3] + " where HeroName='" + heroName + "';";
        UpdateExe(query);
    }

    /// <summary>
    /// 获取英雄属性
    /// </summary>
    /// <param name="equipName"></param>
    /// <returns></returns>
    public int[] GetHeroProperties(string heroName)
    {
        query = "select * from HeroTable where HeroName='" + heroName + "';";
        List<ArrayList> result = SelectMultipleData(query);
        //实例化属性数组
        int[] properties = new int[4];
        if (result == null)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = System.Convert.ToInt32(result[0][i + 2]);
            }
        }

        return properties;
    }

    /// <summary>
    /// 获取当前装备的属性加成
    /// </summary>
    /// <param name="equipName"></param>
    /// <returns></returns>
    public int[] GetEquipProperties(string equipName)
    {
        query = "select * from ShopTable where EquipName='" + equipName + "';";
        List<ArrayList> result = SelectMultipleData(query);
        //实例化属性数组
        int[] properties = new int[4];
        if (result == null)
        {
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = 0;
            }
        }
        else
        {
            for (int i = 0; i < properties.Length; i++)
            {
                properties[i] = System.Convert.ToInt32(result[0][i + 2]);
            }
        }

        return properties;
    }

    /// <summary>
    /// 设置英雄的金钱
    /// </summary>
    /// <param name="heroName"></param>
    /// <param name="heroMoney"></param>
    public void SetHeroMoney(string heroName, int heroMoney)
    {
        query = "update HeroTable set HeroMoney=" + heroMoney.ToString() + " where HeroName='" + heroName + "';";
        UpdateExe(query);
    }

    /// <summary>
    /// 获取英雄所剩金钱
    /// </summary>
    /// <param name="heroName"></param>
    /// <returns></returns>
    public int GetHeroMoney(string heroName)
    {
        query = "select HeroMoney from HeroTable where HeroName='" + heroName + "';";
        object money = SelectSingleData(query);
        return Convert.ToInt32(money);
    }

    /// <summary>
    /// 获取装备的价钱
    /// </summary>
    /// <param name="equipName"></param>
    /// <returns></returns>
    public int GetEquipMoney(string equipName)
    {
        query = "select EquipMoney from ShopTable where EquipName='" + equipName + "';";
        object money = SelectSingleData(query);
        return System.Convert.ToInt32(money);
    }

    /// <summary>
    /// 获取商店中的所有装备
    /// </summary>
    /// <returns></returns>
    public string[] GetShopEquips()
    {
        query = "select EquipName from ShopTable;";
        List<ArrayList> res = SelectMultipleData(query);
        string[] equips = new String[res.Count];

        for (int i = 0; i < equips.Length; i++)
        {
            equips[i] = res[i][0].ToString();
        }
        return equips;
    }

    /// <summary>
    /// 获取英雄名称
    /// </summary>
    public string GetHeroName()
    {
        query = "select HeroName from HeroTable;";
        return SelectSingleData(query).ToString();
    }
}
