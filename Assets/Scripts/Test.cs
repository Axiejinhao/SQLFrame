using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using Mono.Data.Sqlite;

public class Test : MonoBehaviour
{
    private void Start()
    {
        EquipShopFrame.GetInstance().OpenDatabase("ShopDatabase");

        EquipShopFrame.GetInstance().SellEquip("三相之力", "骑着乌龟爆头");
        Debug.Log("OK");
    }

    private void OnApplicationQuit()
    {
        EquipShopFrame.GetInstance().CloseDatabase();
    }
}
