using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSVtest : MonoBehaviour
{
    public static CSVtest instance = null;

    // List 대신 Dictionary를 사용하여 '장비이름'으로 바로 접근하도록
    // Key: 장비이름 (string), Value: 해당 장비의 모든 데이터 (Dictionary<string, object>)
    // 이중 Dictionary
    public Dictionary<string, Dictionary<string, object>> itemDataByName;

    public bool isEnglish;

    private int money = 430;//나중엔 게임 매니저에서 가져올거긴합니다.

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

    private void OnEnable() //한영 패치시 이 게임 오브젝트 비활성화 했다가 다시 키기 //text 표시 gamepbject도 똑같이 Enable에서 데이터를 불러옵시다.
    {
        List<Dictionary<string, object>> Language_data = null;

        // 1. CSVReader를 통해 List 형태로 원본 데이터를 읽습니다.
        if (!isEnglish) { Language_data = CSVReader.Read("datasample2KR"); } //한국어 CSV 파일 읽기
        else{ Language_data = CSVReader.Read("datasample2EN"); } //영문 CSV 파일 읽기

        // 2. 새로운 Dictionary를 만듭니다.
        itemDataByName = new Dictionary<string, Dictionary<string, object>>();

        // 3. List를 순회하며 '장비이름'을 Key로 사용하여 Dictionary에 저장합니다.
        foreach (var entry in Language_data)
        {
            // '장비이름'이 string 타입이라고 가정하고 키로 사용합니다.
            string itemName = entry["장비이름"].ToString();

            // 딕셔너리에 추가합니다.
            itemDataByName.Add(itemName, entry);// 키 밸류값으로 저장
        }
        /*
         itemDataByName["철집게"]["가격"] '아이템 이름'으로 데이터를 빠르게 찾고, 그 안에서 '컬럼 이름'으로 원하는 필드 값을 꺼내기 위해 이중 딕셔너리 구조를 사용
         */
    }

    private void Start()
    {
        // Dictionary를 순회하는 예시
        foreach (var kvp in itemDataByName)
        {
            string name = kvp.Key; // 장비 이름 key
            var dataEntry = kvp.Value; // 해당 장비의 모든 데이터 딕셔너리

            // '가격'을 가져올 때, 해당 필드가 Dictionary에 존재함을 확신한다면 바로 접근합니다.
            Debug.Log($"장비: {name}, 설명: {dataEntry["장비설명"]}, 가격: {(int)dataEntry["가격"]}");
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // 1. 문자열 키값으로 특정 장비의 데이터를 바로 가져오기
            string targetItemName = "철집게"; // 시리얼라이즈 필드로 빼기

            if (itemDataByName.TryGetValue(targetItemName, out Dictionary<string, object> itemData))
            {
                // 2. 해당 장비 데이터에서 가격을 가져옵니다.
                int itemPrice = (int)itemData["가격"]; //시리얼라이즈 필드로 빼기

                if (money >= itemPrice)
                {
                    Debug.Log(money);
                    Debug.Log($"{targetItemName}를 사겠습니다.");
                    Debug.Log(itemPrice);
                    money -= itemPrice;
                    Debug.Log(money);
                }
                else
                {
                    Debug.Log($"돈이 부족합니다. 필요 금액: {itemPrice} 소지금액: {money}");
                }
            }
            else
            {
                Debug.LogError($"CSV 데이터에 '{targetItemName}'(이)가 존재하지 않습니다.");
            }
        }
    }
}
//public class CSVtest : MonoBehaviour
//{
//    List<Dictionary<string, object>> data; //key string, value object
//    //List<Dictionary<string, object>> data2; //key string, value object
//    private int money = 430;
//
//    private void Awake()
//    {
//        data = CSVReader.Read("datasample2"); //datasample 이거 string 으로 시리얼 라이즈 필드로 빼서 읽을 CSV 파일 이름을 외부에서 관리 
//        //data2 = CSVReader.Read("datasample2");
//    }
//    private void Start()
//    {
//        for (int i = 0; i < data.Count; i++)
//        {
//            Debug.Log(data[i]["장비이름"].ToString());   // i번째 데이터의 "장비이름" string 값.
//            Debug.Log(data[i]["장비설명"].ToString());
//            Debug.Log((int)data[i]["가격"]);//형변환
//        }
//    }
//
//    private void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            if (money >= 100)
//            {
//                Debug.Log(money);
//                Debug.Log("철집게를 사겠습니다.");
//                Debug.Log((int)data[1]["가격"]);
//                money -= (int)data[1]["가격"];
//                Debug.Log(money);
//            }
//            else Debug.Log($"돈이 부족합니다. 필요 금액: {(int)data[1]["가격"]} 소지금액: {money}");           
//        }
//    }
//}
