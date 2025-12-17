using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int P_MaxHP = 100; //최대체력
    public int P_CurrentHP; //현재체력
    public int P_Str = 1; //힘
    public int P_Spd = 5; //속도
    public int P_Money = 1000; //소지 돈
    public int P_Maxbag = 100; //가방최대무게

    public int P_Weight = 0; //현재 무게

    public bool P_isEnglish; //한 영문전환

    public int grab_limit = 1;//집을 수 있는 최댓수

    public float P_RemainWeight //남은 가방무게 일단 넣어두기
    {
        get
        {
            return P_Maxbag - P_Weight;
        }
    }

    public void ChangeHP(int HPindecrease)
    {
        P_CurrentHP = Mathf.Clamp(P_CurrentHP + HPindecrease, 0, P_MaxHP);//최대 최소 체력 제한
    }
    //P_Str,P_Spd,P_Money,P_Maxbag은 참조해서 사용

    public bool LoadComplete { get; private set; } = false;

    public static GameManager instance = null;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(WaitForDataManagerAndLoad());
    }

    private IEnumerator WaitForDataManagerAndLoad()
    {
        //우진님 피드백으로 일단 싱글톤 제거할 수 있었던 jsondatamanager제거
        LoadGamedata();

        while (CSV_Database.instance == null)
        {
            yield return null;
        }
        CSV_Database.instance.LoadData();
    }

    public void LoadGamedata()//게임을 다시 켰을때 초기화 값들
    {
        PlayerData loadData = JsonDataManger.LoadformJson();
        P_MaxHP = loadData.MaxHP;
        P_Str = loadData.Str;
        P_Spd = loadData.Spd;
        P_Money = loadData.Money;
        P_Maxbag = loadData.bag;

        P_CurrentHP = P_MaxHP;//체력은 풀로
        P_Weight = 0; //들고 있던 쓰레기 무게 초기화

        P_isEnglish = loadData.isEnglish;
        LoadComplete = true;
        Debug.Log("LoadGamedata");
    }

    public void SaveAllGamedata()
    {
        PlayerData datatosave = new PlayerData
        {
            MaxHP = P_MaxHP,
            Str = P_Str,
            Spd = P_Spd,
            Money = P_Money,
            bag = P_Maxbag,
            isEnglish = P_isEnglish
        };
        JsonDataManger.SavetoJson(datatosave);
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveAllGamedata();//일시 정지되거나 백그라운드로 전환될 때 모든 값 세이브 합니다.
        }
    }
    private void OnApplicationQuit()
    {
        SaveAllGamedata();//꺼지기 전에 모든 값 세이브 합니다.
    }
}
