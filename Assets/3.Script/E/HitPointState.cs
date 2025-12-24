
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HitPointState : MonoBehaviour
{
    ////현재 적용된 콜라이더의 오브젝트를 받아서
    ////오브젝트가 사용된 부분을 구분하고 현 상태로 지정한다
    ////현 상태에 따라 실행하는 부분이 다르다


    //[SerializeField] private SmallTrashAction catchSmall;
    //[SerializeField] private BigTrashAction catchLarge;
    //[SerializeField] private HumanTrashAction catchHuman;
    //[SerializeField] private PlayerWork playerWork;
    //[SerializeField] private PlayerInput input;
    //private float LastGrabTime;
    //[SerializeField] private float TimebetGrab;
    ////이벤트 등록 
    //private void Start()
    //{
    //    //Debug.Log("이벤트 추가 됐어?");
    //    //Debug.Log(GameManager.instance);
    //    TimebetGrab = GameManager.instance.grab_speed;
    //    //TimebetGrab = 1.5f;
    //    LastGrabTime = 0f;
    //}
    //public void Grab()
    //{
    //    TimebetGrab = GameManager.instance.grab_speed;
    //    if (LastGrabTime + TimebetGrab <= Time.time)
    //    {
    //        isTrash();
    //    }
    //}
    //public void isTrash()
    //{
    //    GameObject[] target = playerWork.GetGameObject();
    //    if (GameManager.instance.isPaused)
    //    {
    //        Debug.Log("푸쉬!");
    //        return;
    //    }
        
    //    if (target == null)
    //    {
    //        Debug.Log("null이라서 나갈게");
    //        return;
    //    }
    //    if (target[0] != null && target[0].layer == LayerMask.NameToLayer("NPC"))
    //    {
    //        Debug.Log("NPC 줍기 실행");
    //        catchHuman.CatchMan(target[0]);
    //        return;
    //    }
    //    for (int i = 0; i < target.Length; i++)
    //    {
    //        if (target[i] != null && target[i].layer == LayerMask.NameToLayer("BigTrash"))
    //        {
    //            // Bigtrash 처리
    //            //Debug.Log("큰 쓰레기 발견");
    //            return;
    //        }
    //    }
    //    for (int i = 0; i < target.Length; i++)
    //    {
    //        if (target[i] != null && target[i].layer == LayerMask.NameToLayer("SmallTrash"))
    //        {
    //            LastGrabTime = Time.time;
    //        }
    //    }
    //}
}