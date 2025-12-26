using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSV_GarbageData_Example : MonoBehaviour
{
    private void Update()
    {
        // 스페이스 키가 눌렸을 때만 실행
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // CSV Database의 인스턴스가 존재하는지 확인
            if (CSV_Database.instance == null)
            {
                Debug.LogError("CSV_Database.instance가 Null입니다. 싱글톤 초기화를 확인하세요.");
                return;
            }

            // 데이터가 로드되었는지 확인 (IsLoaded 플래그 사용)
            if (!CSV_Database.instance.IsLoaded)
            {
                Debug.LogWarning("데이터 로딩이 아직 완료되지 않았습니다. 잠시 후 다시 시도하세요.");
                return;
            }

            //여기가 데이터를 빼와서 사용하는 방식입니다.
            if (CSV_Database.instance.GarbageMap.TryGetValue("small_1", out Dictionary<string, object> data))
                //첫번째 key로 이중 dictionary중 첫번째 dictionary를 꺼내옵니다. 
            {
                string s = garbagedata.name.ToString(); //두번째 키를 넣어주세요. enum인 garbagedata로 빼놨습니다.
                object sample;//받아올 value object
                sample = data[s];//dictionary value를 대입시켜 빼옵니다.
                Debug.Log(sample.ToString());//그걸 사용합니다.(string의 경우)

                //예시 2번 int형
                s = garbagedata.Hpdecrease.ToString();
                sample = data[s];
                Debug.Log((int)sample);
            }
            else
            {
                Debug.LogError("GarbageMap에서 키 'small_1'을 찾을 수 없습니다.");
            }
        }
    }
}
