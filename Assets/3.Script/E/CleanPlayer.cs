
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanPlayer : MonoBehaviour
{
    private int currentHP;

    private int[] Hpdecrease = new int[9];
    private int trashNum;

    private bool isTooDirty;

    private void Awake()
    {
        for (int i = 0; i < Hpdecrease.Length; i++)
        {
            string trashName = "small_" + (i);
            StartCoroutine(FindCsvData_co(trashName, i));
        }
    }
    public void Clean(int trashNum)
    {
        
        currentHP = GameManager.instance.P_CurrentHP;
        this.trashNum = trashNum;
        currentHP -= Hpdecrease[trashNum];
            Debug.Log(currentHP+"청결도가 떨어졌어요");
        isTooDirty = currentHP <= 0;
        if (isTooDirty)
        {
            Debug.Log("이건 너무 더럽잖아요!!");
        }
        GameManager.instance.P_CurrentHP = currentHP;

    }
    private IEnumerator FindCsvData_co(string trashName, int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["Hpdecrease"]; ;
            Hpdecrease[i] = (int)sample;
            Debug.Log(trashName + " : " + Hpdecrease[i]);
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 'small_"+i+"'을 찾을 수 없습니다.");
        }
    }
}
