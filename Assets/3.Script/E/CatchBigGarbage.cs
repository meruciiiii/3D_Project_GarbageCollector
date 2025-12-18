
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
    private int[] Strength = new int[6];
    private int trashNum;
    private string trashName;
    [SerializeField] private CleanPlayer cleanPlayer;


    private void Awake()
    {
        for (int i = 0; i < Strength.Length; i++)
        {
            trashName = "large_" + (i);
            StartCoroutine(FindCsvData_co(i));
        }
    }
    //csv에 접근해서 Weight 배열에 큰 쓰레기 들의 무게들 값을 저장 해 둔다.
    private IEnumerator FindCsvData_co(int i)
    {
        while (CSV_Database.instance == null || CSV_Database.instance.GarbageMap == null)
        {
            yield return null;
        }
        if (CSV_Database.instance.GarbageMap.TryGetValue(trashName, out Dictionary<string, object> data))
        {
            object sample = data["requireSrt"]; ;
            Debug.Log(trashName + " : " + (int)sample);
            Strength[i] = (int)sample;
        }
        else
        {
            Debug.LogError("GarbageMap에서 키 'small_1'을 찾을 수 없습니다.");
        }
    }
    public void CatchTrash(GameObject trash)
    {
        //메니저에 있는 현재 상태 변경
        GameManager.instance.isGrabBigGarbage = true;

        trashNum = trash.GetComponent<BigTrash>().getTrashNum();
        this.trash = trash;
        if (CanLift())
        {
            cleanPlayer.Clean(trashNum);
            removeTrash();
        }
        this.trash = null;
    }
    private bool CanLift()
    {
        return true;
    }
    public void removeTrash()
    {
        Destroy(trash);
    }
}
