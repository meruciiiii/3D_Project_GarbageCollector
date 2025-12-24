
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashData : MonoBehaviour
{
    private int[] SmallTrashWeight = new int[10];
    private int[] SmallTrashHpdecrease = new int[10];
    private int[] BigTrashrequiredStrength = new int[8];
    private int[] BigTrashHpdecrease = new int[8];
    private int[] BigTrashWeight = new int[8];
    private void Awake()
    {
        for (int i = 0; i < SmallTrashWeight.Length; i++)
        {
            string trashName = "small_" + (i);
            StartCoroutine(FindSmallTrashData_co(trashName, i));
        }
        for (int i = 0; i < BigTrashrequiredStrength.Length; i++)
        {
            string trashName = "large_" + (i);
            StartCoroutine(FindBigTrashData_co(trashName, i));
            //Debug.Log(trashName + " : " + i);
        }
    }
    private IEnumerator FindSmallTrashData_co(string trashName, int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            //Debug.Log("null 이랍니다");
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["weight"]; ;
            SmallTrashWeight[i] = (int)sample;
            sample = data["Hpdecrease"]; ;
            SmallTrashHpdecrease[i] = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 '" + trashName + "'을 찾을 수 없습니다.");
        }
    }
    private IEnumerator FindBigTrashData_co(string trashName, int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["requireSrt"]; ;
            //Debug.Log(trashName + " : " + (int)sample);
            BigTrashrequiredStrength[i] = (int)sample;
            sample = data["Hpdecrease"]; ;
            BigTrashHpdecrease[i] = (int)sample;
            sample = data["weight"]; ;
            BigTrashWeight[i] = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 '" + trashName + "'을 찾을 수 없습니다.");
        }
    }
    public void setBitGarbageWeight(int trashNum)                                                          //큰 쓰레기의 요구 힘 넣어줄것
    {
        string trashName = "large_" + trashNum;
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["weight"]; ;
            //Debug.Log(trashName + " : " + (int)sample);
            GameManager.instance.BigGarbageWeight = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 '" + trashName + "'을 찾을 수 없습니다.");
        }
        //Debug.Log(GameManager.instance.BigGarbageWeight + " 큰거 무게 확인");
    }
    public int getSmallTrashWeight(int i)
    {
        return SmallTrashWeight[i];
    }
    public int getSmallTrashHpdecrease(int i)
    {
        return SmallTrashHpdecrease[i];
    }
    public int getrequiredStrength(int i)
    {
        return BigTrashrequiredStrength[i];
    }
    public int getBigTrashHpdecrease(int i)
    {
        return BigTrashHpdecrease[i];
    }
    public int getBigTrashWeight(int i)
    {
        return BigTrashWeight[i];
    }
}
