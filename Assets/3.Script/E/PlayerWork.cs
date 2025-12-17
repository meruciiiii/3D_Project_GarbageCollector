
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
    //private HitPointState hitPointState;
    private Vector3 HitPosition;
    private GameObject[] target;
    public LayerMask interactionMask;
    
    
    public HitsSort sorting;
    //[SerializeField] private PlayerInput input;




    public void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerCamera = Camera.main;
        //hitPointState = FindObjectOfType<HitPointState>();


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
        lineRenderer.enabled = false;
        //input.onPickUp += Check;
    }
    private void Update()
    {
        
        Check();
        //DrawLine();
    }
    public void Check()
    {
        // 상호작용 -> raycast
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        RaycastHit[] hits = Physics.RaycastAll(origin, direction, Distance, interactionMask);
        hits = sorting.SortingHits(hits);
        if(hits == null)
        {
            Debug.Log("hits가 널이네요");
            return;
        }
        target = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            target[i] = hits[i].collider.gameObject;
        }
        /*
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, Distance, interactionMask))
        {
            Debug.Log(interactionMask.ToString() + " : 레이어 마스크");
            Debug.Log("쓰레기(큰쓰레기, 작은쓰레기 포함)");
            //hitPointState.HandleInteraction(hit.collider.gameObject);
            target = hit.collider.gameObject;
            HitPosition = hit.point;
        }
        else
        {
            HitPosition = origin + direction * Distance;
        }
         */
    }
    private void DrawLine()
    {
        //라인렌더러 설정  

        //lineRenderer.SetPosition(0, PlayerPosition.position);
        //lineRenderer.SetPosition(1, HitPosition);

        lineRenderer.SetPosition(0, playerCamera.transform.position);
        lineRenderer.SetPosition(1, HitPosition);
    }
    public GameObject[] GetGameObject()
    {
        return target;
    }

    
    //스테이트 에서~
    //private void HandleSmallTrash(GameObject target)
    //{
        
    //}
    //private void HandleBigTrash(GameObject target)
    //{
        
    //}
}
