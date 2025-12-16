
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LineRenderer))]

public class PlayerWork : MonoBehaviour
{
    //플레이어 워크
    // 플레이어가 상호작용 하는 것을 구현
    // 플레이어의 상호작용 종류 - 쓰레기, 판매
    // 상호작용 판단 방법 - raycast
    // 상호작용 거리를 재는것은 프레임 단위로 계속 파악한다
    // 일정 거리 안의 오브젝트를 파악하면 상대 오브젝트 위에 텍스트로 상호작용 여부를 물어본다
    // 파악된 상대
    // 파악이라 하면 레이케스트를 쏘고 그 범위에 있는 쓰레기라고 판단되는 오브젝트들을 가지고 온다.
    // 가지고 오는 능력은 레벨업을 통해서 한 번에 가지고 올 수 있는 개수가 늘어나며
    // 한 번에 처리할 수 있는 범위도 늘어난다.(예시 1개 - 2개 - 3개 - 5개 - 10개)
    //
    private float Distance = 3f;       //플레이어 작업 거리
    private LineRenderer lineRenderer; //플레이어 작업 위치 그리기
    private Camera playerCamera;
    private Vector3 HitPosition;
    public LayerMask interactionMask;


    //State 구현시 State와 연결
    private CatchSmallGarbage catchSmall;


    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerCamera = Camera.main;
        catchSmall = FindObjectOfType<CatchSmallGarbage>();

        //점의 개수
        if (lineRenderer == null)
        {
            Debug.LogError("LineRenderer가 Player에 없습니다!");
            enabled = false;
            return;
        }
        if(playerCamera == null)
        {
            Debug.LogError("MainCamera가 없습니다!");
            enabled = false;
            return;
        }
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;
        lineRenderer.enabled = true;
    }
    private void Update()
    {
        Check();
        DrawLine();
    }
    public void Check()
    {
        // 상호작용 -> raycast
        RaycastHit hit;
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        
        if (Physics.Raycast(origin, direction, out hit, Distance, interactionMask))
        {
            Debug.Log(interactionMask.ToString() + " : 레이어 마스크");
            Debug.Log("쓰레기(큰쓰레기, 작은쓰레기 포함)");
            HandleInteraction(hit.collider.gameObject);
            HitPosition = hit.point;
        }
        else
        {
            HitPosition = origin + direction * Distance;
        }
    }
    private void DrawLine()
    {
        //라인렌더러 설정  

        //lineRenderer.SetPosition(0, PlayerPosition.position);
        //lineRenderer.SetPosition(1, HitPosition);

        lineRenderer.SetPosition(0, playerCamera.transform.position);
        lineRenderer.SetPosition(1, HitPosition);
    }

    private void HandleInteraction(GameObject target)  //스테이트 에서~
    {
        switch (target.layer)
        {
            case int layer when layer == LayerMask.NameToLayer("SmallTrash"):
                //작은 쓰레기 일 경우 구현
                //HandleSmallTrash(target);
                catchSmall.CatchTrash(target);

                break;

            case int layer when layer == LayerMask.NameToLayer("BigTrash"):
                //큰 쓰레기 일 경우 구현
                //HandleBigTrash(target);
                break;
        }
    }
    //스테이트 에서~
    //private void HandleSmallTrash(GameObject target)
    //{
        
    //}
    //private void HandleBigTrash(GameObject target)
    //{
        
    //}
}
