using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum garbagedata
{
    name, // 0 이름
    explaination, // 1 설명
    size, // 2 크기(라지 스몰 S L)
    region, // 3 스폰지역
    weight, // 4 무게
    requireSrt, // 5 요구 힘
    Hpdecrease // 청결도 감소수치
}

public class CSV_Database : MonoBehaviour
{
    public static CSV_Database instance = null;
    private void Awake() //스타트 화면이랑 인게임 씬에서도 쓸것이기에 싱글톤으로 빼겠습니다.
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Debug.Log("이미 CSVtest가 존재합니다.");
            Destroy(gameObject);
        }
    }

    // Key: Key (string), Value: 해당 Key의 모든 데이터 (Dictionary<string, object>)
    // 이중 Dictionary
    public Dictionary<string, Dictionary<string, object>> DataMap;
    public Dictionary<string, Dictionary<string, object>> GarbageMap;
    public bool IsLoaded { get; private set; } = false;

    public void LoadData()
    {
        //첫번째 text UI용 CSV 이중 dictionary
        List<Dictionary<string, object>> Language_data = null; //CSV를 담을 공간입니다.

        // 1. CSVReader를 통해 List 형태로 원본 데이터를 읽습니다.
        if (!GameManager.instance.P_isEnglish) 
        { 
            Language_data = CSVReader.Read("CSV_DataKR");
            Debug.Log("CSV_DataKR 읽기 완료");
        } //Resources 안에 있는 CSV_DataKR 읽기
        else 
        {
            Language_data = CSVReader.Read("CSV_DataEN");
            Debug.Log("CSV_DataEN 읽기 완료");
        } //영문 CSV 파일 읽기

        // 2. 새로운 Dictionary를 만듭니다. 위에선 변수 즉 공간만 선언했었습니다.
        DataMap = new Dictionary<string, Dictionary<string, object>>();

        // 3. List를 순회하며 Key(CSV파일 분류방식으로 Key라고 명명했습니다. 바꿔도 됩니다. 바꾼다면 아래쪽도 같이.)로 Dictionary에 저장합니다.
        foreach (var entry in Language_data)
        {
            // 'num'가 string 타입이라고 가정하고 키로 사용합니다.
            string itemName = entry["num"].ToString();

            // Dictionary에 추가합니다.
            DataMap.Add(itemName, entry);// 키 밸류값으로 저장
        }

        // 두번째 데이터 garbage CSV 이중 dictionary

        List<Dictionary<string, object>> Language_garbage_data = null;

        if (!GameManager.instance.P_isEnglish)
        {
            Language_garbage_data = CSVReader.Read("CSV_GarbageDataKR");
            Debug.Log("CSV_GarbageDataKR 읽기 완료");
        }
        else
        {
            Language_garbage_data = CSVReader.Read("CSV_GarbageDataEN");
            Debug.Log("CSV_GarbageDataEN 읽기 완료");
        }

        GarbageMap = new Dictionary<string, Dictionary<string, object>>();

        foreach (var entry in Language_garbage_data)
        {
            string itemName = entry["num"].ToString();

            GarbageMap.Add(itemName, entry);
        }
        
        IsLoaded = true;
    }

    //피드백 요구 메서드 key값만 주면 바로 값 내뱉을 수 있게
    //Garbage 데이터입니다.
    public string getname(string key)
    {
        string value = "";
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = data["name"].ToString();
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public string getexplaination(string key)
    {
        string value = "";
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = data["explaination"].ToString();
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public string getsize(string key)
    {
        string value = "";
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = data["size"].ToString();
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public string getregion(string key)
    {
        string value = "";
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = data["region"].ToString();
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public int getweight(string key)
    {
        int value = 0;
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = (int)data["weight"];
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public int getrequireSrt(string key)
    {
        int value = 0;
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = (int)data["requireSrt"];
        else Debug.Log("key가 없습니다.");
        return value;
    }
    public int getHpdecrease(string key)
    {
        int value = 0;
        if (GarbageMap.TryGetValue(key, out Dictionary<string, object> data)) value = (int)data["Hpdecrease"];
        else Debug.Log("key가 없습니다.");
        return value;
    }
}