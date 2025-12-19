
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchBigGarbage : MonoBehaviour
{
    // 큰 쓰레기의 경우 들어올리는 작업을 하면 상태가 변화한다.
    // raycast로 받은 배열중에 존재하면 바로 큰 쓰레기를 집어듭니다.
    // 힘에 따라서 들어 올릴 수 있는 개수가 달라집니다.
    //포지션과 로테이션이 다라집니다
    // 내려놓는 작업을 하면 리지드 바디가 생성됩니다
    // 들어올렸을 때는 안보이게 만들고
    // 들어올린 상태는 프리팹화 한 값을 보여줍니다
    // 내려 놓으면
    private GameObject trash;
    private int[] Strength = new int[5];
    private int trashNum;
    private int bigGarbageWeight;
    private string trashName;
    private int nowStrength;
    [SerializeField] private CleanPlayer cleanPlayer;


    private void Awake()
    {
        for (int i = 0; i < Strength.Length; i++)
        {
            trashName = "large_" + (i);
            StartCoroutine(FindCsvData_co(trashName, i));
            //Debug.Log(trashName + " : " + i);
        }
    }
    //csv에 requireSrt로 접근해서 Strength 배열에 큰 쓰레기 들의 무게들 값을 저장 해 둔다.
    private IEnumerator FindCsvData_co(string trashName, int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["requireSrt"]; ;
            //Debug.Log(trashName + " : " + (int)sample);
            Strength[i] = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 '"+ trashName + "'을 찾을 수 없습니다.");
        }
    }
    public void CatchTrash(GameObject trash)
    {
        //메니저에 있는 현재 상태 변경
        GameManager.instance.isGrabBigGarbage = true;
        this.trash = trash;
        if (CanLift())
        {
            cleanPlayer.Clean(trashNum);
            setBitGarbageWeight();
            removeTrash();
        }
        this.trash = null;
    }
    private bool CanLift()
    {
        trashNum = trash.GetComponent<BigTrash>().getTrashNum();
        nowStrength = GameManager.instance.P_Str;
        if(nowStrength>= Strength[trashNum])
            return true; //들 수 있다.
        else
            return false; //들 수 없다.

    }
    private void removeTrash()
    {
        trash.SetActive(false);
    }
    private void setBitGarbageWeight()
    {
        trashName = "large_" + trashNum;
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["weight"]; ;
            //Debug.Log(trashName + " : " + (int)sample);
            bigGarbageWeight = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 '" + trashName + "'을 찾을 수 없습니다.");
        }
        GameManager.instance.BigGarbageWeight = bigGarbageWeight;
    }
}
