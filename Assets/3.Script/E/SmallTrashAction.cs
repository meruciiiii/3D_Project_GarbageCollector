
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTrashAction : MonoBehaviour
{
    public bool CanAdd(int weight)
    {
        if (GameManager.instance.P_Maxbag >= GameManager.instance.P_Weight + weight)
        {
            return true;
        }
        else
        {
            Debug.Log("이 것을 담기에는 용량이 초과됩니다.");
            return false;
        }
        ///과거
        /// 최대 가방 데이터를 가져온다
        ///BagWeight = GameManager.instance.P_Weight;
        ///Debug.Log(BagWeight + " : 게임매니저에 저장된 현재 무게");
        ///
        /// 최대 가방 데이터를 가져온다
        ///MaxWeight = GameManager.instance.P_Maxbag;
        ///Debug.Log(MaxWeight + " : 게임매니저에 저장된 최대 보유 무게");
        ///
        /// 쓰레기의 무게 데이터를 가져온다
        ///trashNum = trash.GetComponent<Trash>().getTrashNum();
        ///Debug.Log(trashNum + " : 번 쓰레기");
        ///
        /// 현재 가방 무게에 쓰레기의 무게를 더한다
        ///if (MaxWeight >= BagWeight + Weight[trashNum])
        ///{
        ///    BagWeight += Weight[trashNum];
        ///}
        ///else
        ///{
        ///    Debug.Log("이 것을 담기에는 용량이 초과됩니다.");
        ///    return false;
        ///}
        ///Debug.Log(BagWeight + " : 추가된 현재 무게");
        ///GameManager.instance.P_Weight = BagWeight;
        /// 제대로 저장 되었는지 확인
        ///Debug.Log(GameManager.instance.P_Weight + " : 게임매니저에 저장된 현재 무게");
    }
    public void Add(int weight)
    {
        GameManager.instance.P_Weight += GameManager.instance.P_Weight + weight;
    }
}
