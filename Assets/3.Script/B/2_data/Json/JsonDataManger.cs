using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[Serializable]
public class PlayerData
{
    public int MaxHP = 100;
    public int Str = 1;
    public int Spd = 5;
    public int Money = 0;
    public int bag = 5000;

    public int grab = 1;
    public float grabspd = 1.5f;

    public bool isEnglish = false;
    public bool intro = true;
}
public static class JsonDataManger
{
    private static string filename = "Player_data.json";
    private static string path => Path.Combine(Application.persistentDataPath, filename);
    // 호출되는 순간 경로를 계산해서 반환함

    public static void SavetoJson(PlayerData data)
    {
        string Jsondata = JsonUtility.ToJson(data);

        File.WriteAllText(path, Jsondata); //경로 파일 안에 json파일로 저장
        Debug.Log("Playerdata Wirte완료");
    }

    public static PlayerData LoadformJson()//데이터 불러오기 메서드
    {
        if (!File.Exists(path))//파일이 경로에 없다면
        {
            PlayerData newPlayerdata = new PlayerData();
            SavetoJson(newPlayerdata);
            return newPlayerdata;
        }
        string Jsondata = File.ReadAllText(path);
        PlayerData jsonPlayerdata = JsonUtility.FromJson<PlayerData>(Jsondata);
        Debug.Log("Playerdatajson Load완료");
        return jsonPlayerdata;
    }
}
