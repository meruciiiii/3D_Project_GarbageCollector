
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatchSmallGarbage : MonoBehaviour
{
    //작은 쓰레기 일 경우 다음이 진행된다.
    //작은 쓰레기의 경우 해당 오브젝트가 사라진다.
    //작은 쓰레기의 경우 인벤토리에 들어간다.
    //인벤토리에 들어가면 인벤토리의 무게가 늘어난다.
    public GameObject trash;
    private int trashNum;
    private int[] Weight = new int[9];
    private int BagWeight;
    private int MaxWeight;
    private void Awake()
    {
        for(int i = 0; i < Weight.Length; i++)
        {
            string trashName = "small_"+(i);
            StartCoroutine(FindCsvData_co(trashName,i));
        }
        
    }
    public void CatchTrash(GameObject trash)
    {
        GameManager.instance.P_Maxbag = 5000;
        this.trash = trash;
        if (AddBackpack())
        {
            removeTrash();
        }
    }
    public void removeTrash()
    {
        Destroy(trash);
        //cachedObjects.Remove(trash); 
    }
    public bool AddBackpack()
    {
        // 최대 가방 데이터를 가져온다
        BagWeight = GameManager.instance.P_Weight;
        Debug.Log(BagWeight + " : 게임매니저에 저장된 현재 무게");

        // 최대 가방 데이터를 가져온다
        MaxWeight = GameManager.instance.P_Maxbag;
        Debug.Log(MaxWeight + " : 게임매니저에 저장된 최대 보유 무게");

        // 쓰레기의 무게 데이터를 가져온다
        trashNum = trash.GetComponent<SmallTrash>().getTrashNum();
        Debug.Log(trashNum + " : 번 쓰레기");

        // 현재 가방 무게에 쓰레기의 무게를 더한다
        if (MaxWeight >= BagWeight + Weight[trashNum])
        {
            BagWeight += Weight[trashNum];
        }
        else
        {
            Debug.Log("이 것을 담기에는 용량이 초과됩니다.");
            return false;
        }
        Debug.Log(BagWeight + " : 추가된 현재 무게");
        GameManager.instance.P_Weight = BagWeight;
        // 제대로 저장 되었는지 확인
        Debug.Log(GameManager.instance.P_Weight + " : 게임매니저에 저장된 현재 무게");
        return true;
    }
    private IEnumerator FindCsvData_co(string trashName, int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            Debug.Log("null 이랍니다");
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["weight"]; ;
            Debug.Log(trashName+" : "+(int)sample);
            Weight[i] = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 'small_1'을 찾을 수 없습니다.");
        }
    }
}
