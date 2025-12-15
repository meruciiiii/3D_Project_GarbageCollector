using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LitJson;
using System.IO;

[Serializable]
public class PlayerData
{
    public int MaxHP;
    public int Str;
    public int Spd;
    public int Money;
    public int bag;
}
public class DataManger : MonoBehaviour
{
    public static DataManger instance;

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
            //제이슨 저장해주세요.
        }
    }

    public void SavetoJson(PlayerData data)
    {
        string Jsondata = JsonMapper.ToJson(data);

        File.WriteAllText(path, Jsondata); //경로 파일 안에 json파일로 저장
    }
}
