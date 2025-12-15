using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVdataExample : MonoBehaviour
{
    private int moneyexample = 910;
    private int itemcount = 0;
    //moneyexample, itemcount는 GameManager에서 나중에 관리할겁니다.

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            string inputkey = "철집게";
            if (CSV_Database.instance.DataMap.TryGetValue(inputkey, out Dictionary<string, object> Data))
            {
                int itemPrice = (int)Data["Value_Price"];
                if(moneyexample >= itemPrice)
                {
                    Debug.Log($"현재 재화: {moneyexample}");
                    moneyexample -= itemPrice;
                    itemcount++;
                    Debug.Log(inputkey);
                    Debug.Log($"구매상품 이름: {Data["Keyname"]}");
                    Debug.Log($"구매상품 설명: {Data["Value_Content"]}");
                    Debug.Log($"사용금액 금액: {(int)Data["Value_Price"]}");
                    Debug.Log("아이템을 구매했습니다.");
                    Debug.Log($"남은 재화: {moneyexample}");
                    Debug.Log($"아이템 소지수: {itemcount}");
                    Debug.Log("--------------------------------------");
                }
                else
                {
                    Debug.Log($"현재 재화: {moneyexample}");
                    Debug.Log(inputkey);
                    Debug.Log($"구매상품 이름: {Data["Keyname"]}");
                    Debug.Log($"구매상품 설명: {Data["Value_Content"]}");
                    Debug.Log($"사용금액 금액: {(int)Data["Value_Price"]}");
                    Debug.Log("잔액이 부족하여 아이템을 구매하지 못했습니다.");
                    Debug.Log($"남은 재화: {moneyexample}");
                    Debug.Log($"아이템 소지수: {itemcount}");
                    Debug.Log("--------------------------------------");
                }
            }
        }
    }
}
