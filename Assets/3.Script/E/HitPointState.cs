
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class HitPointState : MonoBehaviour
{
    //현재 적용된 콜라이더의 오브젝트를 받아서
    //오브젝트가 사용된 부분을 구분하고 현 상태로 지정한다
    //현 상태에 따라 실행하는 부분이 다르다


    [SerializeField] private CatchSmallGarbage catchSmall;
    [SerializeField] private CatchBigGarbage catchLarge;
    [SerializeField] private PlayerWork playerWork;
    [SerializeField] private PlayerInput input;


    //이벤트 등록 
    private void Start()
    {
        input.onPickUp += isTrash;
        //Debug.Log("이벤트 추가 됐어?");
    }
    private void Awake()
    {
    }
    public void isTrash()
    {
        GameObject[] target = playerWork.GetGameObject();
        if (target == null)
        {
            Debug.Log("null이라서 나갈게");
            return;
        }
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] != null && target[i].layer == LayerMask.NameToLayer("BigTrash"))
            {
                // Bigtrash 처리
                Debug.Log("큰 쓰레기 발견");
                catchLarge.CatchTrash(target[i]);
                return;
            }
        }
        for (int i = 0; i < target.Length; i++)
        {
            if (target[i] != null && target[i].layer == LayerMask.NameToLayer("SmallTrash"))
            {
                catchSmall.CatchTrash(target[i]);
            }
        }
    }
    public void HandleInteraction(GameObject target)
    {
        switch (target.layer)
        {
            case int layer when layer == LayerMask.NameToLayer("SmallTrash"):
                //작은 쓰레기 일 경우 구현
                //catchSmall.CatchTrash(target);

                break;

            case int layer when layer == LayerMask.NameToLayer("BigTrash"):
                //큰 쓰레기 일 경우 구현
                break;
        }
    }
}
