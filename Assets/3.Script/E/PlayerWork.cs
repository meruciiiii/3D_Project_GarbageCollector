
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//[RequireComponent(typeof(LineRenderer))]

public class PlayerWork : MonoBehaviour
{
    ///플레이어 워크
    /// 플레이어가 상호작용 하는 것을 구현
    /// 플레이어의 상호작용 종류 - 쓰레기, 판매
    /// 상호작용 판단 방법 - raycast
    /// 상호작용 거리를 재는것은 프레임 단위로 계속 파악한다
    /// 일정 거리 안의 오브젝트를 파악하면 상대 오브젝트 위에 텍스트로 상호작용 여부를 물어본다
    /// 파악된 상대
    /// 파악이라 하면 레이케스트를 쏘고 그 범위에 있는 쓰레기라고 판단되는 오브젝트들을 가지고 온다.
    /// 가지고 오는 능력은 레벨업을 통해서 한 번에 가지고 올 수 있는 개수가 늘어나며
    /// 한 번에 처리할 수 있는 범위도 늘어난다.(예시 1개 - 2개 - 3개 - 5개 - 10개)
    ///
    private float Distance = 3f;       //플레이어 작업 거리
    private float radius = 0.2f;                // raycast 두께 - 청소 가능 범위
    //private LineRenderer lineRenderer; //플레이어 작업 위치 그리기
    private Camera playerCamera;
    public CleanPlayer cleanPlayer;
    //private HitPointState hitPointState;
    private Vector3 HitPosition;
    private GameObject[] target = Array.Empty<GameObject>();
    public LayerMask interactionMask;

    private Vector3 origin;
    private Vector3 direction;
    private RaycastHit[] hits;
    private RaycastHit hit;


    private bool isPicking;
    private float pickInterval;
    private float nextPickTime;


    [SerializeField] private HitsSort sorting;
    [SerializeField] private PlayerInput input;
    [SerializeField] public SmallTrashAction smallTrashAction;
    [SerializeField] public BigTrashAction bigTrashAction;
    [SerializeField] public HumanTrashAction humanTrashAction;
    [SerializeField] public PlayerController controller;
    [SerializeField] private UIManager uIManager;
    [Header("UI 연결")]
    [SerializeField] private ThashInfo trashInfoUI;


    public void Awake()
    {
        //lineRenderer = GetComponent<LineRenderer>();
        playerCamera = Camera.main;
        //hitPointState = FindObjectOfType<HitPointState>();

        if (playerCamera == null)
        {
            //Debug.LogError("MainCamera가 없습니다!");
            enabled = false;
            return;
        }
        input.onPickUp += () => isPicking = true;
        input.offPickUp += () => isPicking = false;
        //input.onPickUp += () => Interact(this);
        if (!TryGetComponent<PlayerController>(out controller))
        {
            //Debug.Log("Failed to call controller");
            return;
        }
    }
    private void Start()
    {
        pickInterval = (float)GameManager.instance.grab_speed;
    }
    private void Update()
    {
        Detect();
        HandleContinuousPick();
    }
    private void Detect()
    {
        origin = playerCamera.transform.position;
        direction = playerCamera.transform.forward;
        radius = GameManager.instance.grab_range;

        hits = Physics.SphereCastAll(origin, radius, direction, Distance, interactionMask);
        UpdateAnchorPoint();
        SelectTargets();
    }
    private void HandleContinuousPick()
    {
        if (!isPicking)
            return;

        if (Time.time < nextPickTime)
            return;

        //nextPickTime = Time.time + pickInterval;
        //Debug.Log(nextPickTime + " : nextPickTime = 다음 선택 가능 시간");

        //pickTimer = 0f;
        Interact(this);
    }
    private void UpdateAnchorPoint()
    {
        if (Physics.Raycast(origin, direction, out hit, Distance, interactionMask))
        {
            HitPosition = hit.point;
            ///Debug.Log(hit.collider.gameObject.layer.ToString());
            ///if (hit.collider.gameObject.layer.Equals(LayerMask.NameToLayer("NPC")))
            ///{
            ///    hits = new RaycastHit[1];
            ///    hits[0] = hit;
            ///    //아웃라인 및 머테리얼 부분 추후 수정
            ///    //sorting.HumanCheck(hits);
            ///    target = new GameObject[1];
            ///    target[0] = hits[0].collider.gameObject;
            ///    //Debug.Log("target ? " + target[0].name);
            ///    return;
            ///}
        }
        else
        {
            HitPosition = origin + direction * Distance;
        }
    }
    public void SelectTargets()
    {
        int layer;
        if (hit.collider == null)
        {
            sorting.lastHitsOffOutline();
            target = Array.Empty<GameObject>();
            if (trashInfoUI != null) trashInfoUI.HideInfo();
            return;
        }
        else
        {
            layer = hit.collider.gameObject.layer;
        }
        hits = sorting.SortingHits(hits, hit, HitPosition, layer);
        if (hits.Length == 0)
        {
            sorting.lastHitsOffOutline();
            target = Array.Empty<GameObject>();
            //Debug.Log("hits가 널이네요");
            if (trashInfoUI != null) trashInfoUI.HideInfo();
            return;
        }
        target = new GameObject[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            target[i] = hits[i].collider.gameObject;
        }
        if (trashInfoUI != null && target.Length > 0)
        {
            trashInfoUI.UpdateTooltip(target);
        }
    }
    private void Interact(PlayerWork player)
    {

        if (humanTrashAction.IsHolding)
        {
            if (!Cursor.visible)
            {
                humanTrashAction.DrobGarbage();

            }
            controller.Calc_Speed();
            return;
        }
        if (bigTrashAction.IsHolding)
        {
            if (!Cursor.visible)
            {
                bigTrashAction.DrobGarbage();
            }
            controller.Calc_Speed();
            return;
        }
        if (target == null || target.Length == 0) return;
        ///if(target[0].TryGetComponent<IInteractable>(out IInteractable interactable))
        ///{
        ///    interactable.Interact();
        ///}
        ///else
        ///{
        ///    Debug.Log("IInteractable 컴포넌트가 없습니다.");
        ///}
        foreach (GameObject obj in target)
        {
            if (obj.TryGetComponent<IInteractable>(out IInteractable interactable))
            {
                interactable.Interact(player);
            }
            else
            {
                //Debug.Log("IInteractable 컴포넌트가 없습니다.");
            }
            nextPickTime = Time.time + pickInterval;
            uIManager.change_Value();
            sorting.lastHitsOffOutline();
            sorting.CatchandEmpty();
        }
        controller.Calc_Speed();
        if (bigTrashAction.IsHolding || humanTrashAction.IsHolding)
        {
            isPicking = false;
        }
    }
    public GameObject[] GetGameObject()
    {
        //Debug.Log("target ? " + target[0].name);
        return target;
    }
    public void SetPickInterval()
    {
        pickInterval = GameManager.instance.grab_speed;
    }
    public void startPick()
    {
        Interact(this);
    }
}