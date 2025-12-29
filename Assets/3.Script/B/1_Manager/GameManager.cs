using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int P_MaxHP = 100; //최대체력
    private int p_CurrentHP; //현재 체력
    public int P_CurrentHP //차량 부딫힐때 필요
    {
        get => p_CurrentHP;
        set
        {
            // 체력 제한 로직을 여기에, ChangeHP대용, 음수값 제한도 여기서 처리됌
            p_CurrentHP = Mathf.Clamp(value, 0, P_MaxHP);

            // 값이 바뀔 때마다 UIManager에 알림
            if (UIManager.instance != null)
            {
                UIManager.instance.change_Value();
            }
        }
    }
    public int P_Str = 1; //힘
    public int P_Spd = 5; //속도

    public int P_Money = 0; //소지 돈
    public int P_Maxbag = 10000; //가방최대무게

    public event Action OnWeightIncreased; // 무게가 늘어날 때만 쏴주는 전광판 신호 aimcooldown에서 사용하겠습니다.

    private int p_Weight = 0; // 실제 데이터가 담기는 내부 저장소
    public int P_Weight
    {
        get => p_Weight; // 누군가 무게를 물어보면 p_Weight 값을 알려줌
        set
        {
            // +=, -=, = 등으로 값이 들어오면 무조건 여기(set)를 거칩니다.
            if (value > p_Weight)
            {
                // 값이 이전보다 커졌다면(줍기 성공)
                p_Weight = value;
                OnWeightIncreased?.Invoke(); // 구독 중인 UI들에게 "게이지 틀어!"라고 신호 보냄
            }
            else
            {
                // 값이 줄어들거나(정산) 같으면 그냥 값만 저장
                p_Weight = value;
            }
        }
    }

    public bool P_isEnglish; //한 영문전환
    public bool P_intro = true; //게임 첫 시작시 intro
    public bool GameClear = false;

    public int grab_limit = 1;//집을 수 있는 최댓수
    public float grab_range = 0.15f;//집을 수 있는 범위 임의값 float 0.15f
    public float grab_speed = 1.5f;//집는 속도

    //큰 쓰레기 전용 값 3종류
    public bool isGrabBigGarbage = false;
    public int BigGarbageWeight = 0;
    public int BigneedStr = 0;

    public float P_RemainWeight //남은 가방무게 일단 넣어두기
    {
        get
        {
            return P_Maxbag - P_Weight;
        }
    }

    public int Current_Area = 1;

    public void ChangeHP(int HPindecrease)
    {
        P_CurrentHP += HPindecrease;
    }

    public bool LoadComplete { get; private set; } = false;

    public bool isPaused = false;//퍼즈시에 쓰레기 못줍게, 정산, 업그레이드창 안뜨게

    public static GameManager instance = null;
    private void Awake()
    {
        if(instance == null)
        {
            //Debug.Log("instance생성");
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

        grab_limit = loadData.grab;
        grab_speed = loadData.grabspd;
        grab_range = loadData.grab_range;

        P_CurrentHP = P_MaxHP;//체력은 풀로
        P_Weight = 0; //들고 있던 쓰레기 무게 초기화

        P_isEnglish = loadData.isEnglish;
        P_intro = loadData.intro;

        LoadComplete = true;
        //Debug.Log("LoadGamedata");
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
            isEnglish = P_isEnglish,
            grab = grab_limit,
            grabspd = grab_speed,
            grab_range = this.grab_range,
            intro = P_intro
        };
        JsonDataManger.SavetoJson(datatosave);
    }
    public void ResetGameData()
    {
        P_MaxHP = 100;
        P_CurrentHP = 100;
        P_Str = 1;
        P_Spd = 5;
        P_Money = 0;
        P_Maxbag = 10000;
        P_Weight = 0;

        grab_limit = 1;
        grab_speed = 1.5f;
        grab_range = 0.15f;

        isGrabBigGarbage = false;
        BigGarbageWeight = 0;

        P_intro = true;

        Debug.Log("모든 게임 데이터가 초기값으로 설정되었습니다.");

        SaveAllGamedata(); 
    }

    public void Cheatdata()
    {
        P_MaxHP = 100;
        P_CurrentHP = 100;
        P_Str = 1;
        P_Spd = 5;
        P_Money = 999999999;
        P_Maxbag = 10000;
        P_Weight = 0;

        grab_limit = 1;
        grab_speed = 1.5f;
        grab_range = 0.15f;

        Debug.Log("cheat.");

        SaveAllGamedata();
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
