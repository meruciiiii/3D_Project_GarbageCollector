using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int P_MaxHP = 100; //최대체력
    public int P_CurrentHP = 100; //현재체력
    public int P_Str = 1; //현재 힘
    public int P_Spd = 5; //현재 속도
    public int P_Money = 1000; //현재 돈
    public int P_Maxbag = 100; //현재 가방최대무게
    public int P_weight = 0;
    public bool P_isEnglish;
    public int P_Remainweight //남은 가방무게 일단 넣어두기
    {
        get
        {
            return P_Maxbag - P_weight;
        }
    }
    public bool IsInitialLoadComplete { get; private set; } = false;

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
        // DataManger 인스턴스가 생성될 때까지 기다립니다.
        while (DataManger.instance == null)
        {
            yield return null;
        }
        // DataManger가 준비된 후에 로드를 시도
        LoadGamedata();

        // CSV_Database도 초기 로드를 시작하도록 명령 (CSV_Database가 GameManager를 기다린다고 가정)
        if (CSV_Database.instance != null)
        {
            CSV_Database.instance.LoadData();
        }
    }

    public void LoadGamedata()
    {
        if (DataManger.instance == null) return;
        PlayerData loadData = DataManger.instance.LoadformJson();
        P_MaxHP = loadData.MaxHP;
        P_Str = loadData.Str;
        P_Spd = loadData.Spd;
        P_Money = loadData.Money;
        P_Maxbag = loadData.bag;

        P_CurrentHP = P_MaxHP;
        P_weight = 0; //들고 있던 쓰레기 무게 초기화

        P_isEnglish = loadData.isEnglish;
        IsInitialLoadComplete = true;
    }

    public void SaveAllGamedata()
    {
        if (DataManger.instance == null) return;
        PlayerData datatosave = new PlayerData
        {
            MaxHP = P_MaxHP,
            Str = P_Str,
            Spd = P_Spd,
            Money = P_Money,
            bag = P_Maxbag,
            isEnglish = P_isEnglish
        };
        DataManger.instance.SetPlayerdata(datatosave);
    }

    public void ChangeHP(int HPindecrease)
    {
        P_CurrentHP = Mathf.Clamp(P_CurrentHP + HPindecrease, 0, P_MaxHP);//최대 최소 체력 제한
    }

    //P_Str,P_Spd,P_Money,P_Maxbag은 그냥 사용  

    private void OnApplicationQuit()
    {
        SaveAllGamedata();//꺼지기 전에 모든 값 세이브 합니다.
    }
}
