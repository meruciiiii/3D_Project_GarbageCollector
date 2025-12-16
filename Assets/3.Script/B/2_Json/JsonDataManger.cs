using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[Serializable]
public class PlayerData
{
    public int MaxHP = 100;
    public int Str = 1;
    public int Spd = 5;
    public int Money = 1000;
    public int bag = 100;
    public bool isEnglish = false;
}
public class JsonDataManger : MonoBehaviour
{
    public static JsonDataManger instance;

    private string filename = "Player_data.json";
    private static string path; //파일 저장 경로

    private void Awake()
    {
        instance = this;
        path = Path.Combine(Application.persistentDataPath, filename);
        //파일 경로에 파일 이름을 합쳐서 string화
        //persistentDataPath 경로
        //C:\Users\[user name]\AppData\LocalLow\[company name]\[product name]
    }

    private void Start()
    {
        if (!File.Exists(path))
        {
            SavetoJson(new PlayerData());
        }
    }

    public void SavetoJson(PlayerData data)
    {
        string Jsondata = JsonMapper.ToJson(data);

        File.WriteAllText(path, Jsondata); //경로 파일 안에 json파일로 저장
        Debug.Log("Playerdata Wirte완료");
    }

    public PlayerData LoadformJson()//데이터 불러오기 메서드
    {
        if (!File.Exists(path))
        {
            PlayerData newPlayerdata = new PlayerData();
            return newPlayerdata;
        }
        string Jsondata = File.ReadAllText(path);
        PlayerData jsonPlayerdata = JsonMapper.ToObject<PlayerData>(Jsondata);
        Debug.Log("Playerdatajson Load완료");
        return jsonPlayerdata;
    }

    public void SetPlayerdata(PlayerData newData)
    {
        SavetoJson(newData);
        Debug.Log("데이터 저장 완료");
    }
}
