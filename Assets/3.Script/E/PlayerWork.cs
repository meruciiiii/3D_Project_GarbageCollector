
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(LineRenderer))]

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
    private float radius = 0.2f;                // raycast 두께 - 청소 가능 범위
    //private LineRenderer lineRenderer; //플레이어 작업 위치 그리기
    private Camera playerCamera;
    //private HitPointState hitPointState;
    private Vector3 HitPosition;
    private GameObject[] target;
    public LayerMask interactionMask;
    
    
    public HitsSort sorting;
    //[SerializeField] private PlayerInput input;


    [SerializeField] private PlayerInput input;


    public void Awake()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        playerCamera = Camera.main;
        //hitPointState = FindObjectOfType<HitPointState>();

        if(playerCamera == null)
        {
            Debug.LogError("MainCamera가 없습니다!");
            enabled = false;
            return;
        }
        input.onPickUp += Check;
    }
    private void Update()
    {
        
        Check();
        //DrawLine();
    }
    public void Check()
    {
        // 상호작용 -> raycast
        target = null;
        Vector3 origin = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;
        radius = GameManager.instance.grab_range;
        RaycastHit[] hits = Physics.SphereCastAll(origin, radius, direction, Distance, interactionMask);
        RaycastHit hit;
        if(Physics.Raycast(origin, direction, out hit, Distance, interactionMask))
        {
            HitPosition = hit.point;
        }
        else
        {
            sorting.lastHitsOffOutline();
            return;
        }
        hits = sorting.SortingHits(hits, HitPosition, hit.collider.gameObject.layer);
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
    }
    public GameObject[] GetGameObject()
    {
        return target;
    }
}
