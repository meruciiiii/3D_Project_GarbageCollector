using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_Database : MonoBehaviour
{
    public static CSV_Database instance = null;

    // Key: Key (string), Value: 해당 Key의 모든 데이터 (Dictionary<string, object>)
    // 이중 Dictionary
    public Dictionary<string, Dictionary<string, object>> DataMap;
    public bool IsLoaded { get; private set; } = false;

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

    public void LoadData()
    {
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
            // 'Key'가 string 타입이라고 가정하고 키로 사용합니다.
            string itemName = entry["Key"].ToString();

            // Dictionary에 추가합니다.
            DataMap.Add(itemName, entry);// 키 밸류값으로 저장
        }
        IsLoaded = true;
    }

    /*
       // Dictionary를 순회하는 예시
       foreach (var kvp in DataMap)
       {
           string Key = kvp.Key; // Key
           var dataEntry = kvp.Value; // 해당 Key의 모든 데이터 딕셔너리

           Debug.Log($"Key: {Key}, Keyname: {dataEntry["Keyname"]}, Value_Content: {dataEntry["Value_Content"]}, Value_Price: {(int)dataEntry["Value_Price"]}");
       }
       */
}