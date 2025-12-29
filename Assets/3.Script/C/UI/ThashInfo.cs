using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 연결 설정")]
    [Tooltip("텍스트와 패널을 포함하는 부모 오브젝트 (배경 패널)")]
    [SerializeField] private GameObject uiPanel;

    [Tooltip("쓰레기 이름이 표시될 텍스트")]
    [SerializeField] private Text nameText;

    [Tooltip("무게, 힘, 청결도 등 상세 정보가 표시될 텍스트")]
    [SerializeField] private Text infoText;

    [Header("감지 설정")]
    [SerializeField] private float checkDistance = 5.0f;
    [SerializeField] private LayerMask trashLayerMask;

    private Camera mainCamera;
    private Trash lastHitTrash = null;

    // [최적화] 문자열 할당(Garbage Collection)을 줄이기 위해 StringBuilder를 재사용합니다.
    private StringBuilder infoBuilder = new StringBuilder();

    private void Start()
    {
        mainCamera = Camera.main;
        HideInfo();
    }

    private void Update()
    {
        CheckObjectInFront();
    }

    private void CheckObjectInFront()
    {
        if (mainCamera == null) return;

        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, checkDistance, trashLayerMask))
        {
            // [성능 노트] GetComponentInParent는 가볍지 않으므로, 추후 최적화가 필요하다면 캐싱 방식을 고려할 수 있습니다.
            Trash trash = hit.collider.GetComponentInParent<Trash>();

            if (trash != null)
            {
                ShowInfo(trash);
                return;
            }
        }

        HideInfo();
    }

    private void ShowInfo(Trash trash)
    {
        // 텍스트가 이미 켜져 있고, 바라보는 쓰레기가 같다면 연산을 건너뜁니다.
        if (uiPanel != null && uiPanel.activeSelf && lastHitTrash == trash) return;

        lastHitTrash = trash;

        // 1. 키값 생성 로직
        string keyPrefix = "";
        switch (trash.Size)
        {
            case Trash.TrashSize.Small: keyPrefix = "small_"; break;
            case Trash.TrashSize.Large: keyPrefix = "large_"; break;
            case Trash.TrashSize.Human: keyPrefix = "human_"; break;
        }
        string finalKey = keyPrefix + trash.TrashNum;

        // 2. 데이터 유효성 검사 (매니저들이 로드되지 않았을 경우 방어)
        if (CSV_Database.instance == null || GameManager.instance == null) return;

        string tName = CSV_Database.instance.getname(finalKey);
        int tWeight = CSV_Database.instance.getweight(finalKey);
        int reqStr = CSV_Database.instance.getrequireStr(finalKey);
        int hpCost = CSV_Database.instance.getHpdecrease(finalKey);
        int currentStr = GameManager.instance.P_Str;


        if (nameText != null)
        {
            nameText.text = tName;
        }

        // (B) 상세 정보 텍스트 설정 (StringBuilder 활용)
        if (infoText != null)
        {
            infoBuilder.Clear();

            // 무게
            if (!GameManager.instance.P_isEnglish) infoBuilder.AppendLine($"무게: {tWeight} g");
            else infoBuilder.AppendLine($"weight: {tWeight} g");
            
            // 힘 요구량 비교 및 색상 처리
            if (currentStr >= reqStr)
            {
                if (!GameManager.instance.P_isEnglish) infoBuilder.AppendLine($"필요 힘: {reqStr}");
                else infoBuilder.AppendLine($"reqiure str: {reqStr}");
            }
            else
            {
                    // 부족할 때 빨간색 경고
                if (!GameManager.instance.P_isEnglish) 
                {
                    infoBuilder.Append("<color=red>힘 부족! (");
                    infoBuilder.Append(currentStr);
                    infoBuilder.Append("/");
                    infoBuilder.Append(reqStr);
                    infoBuilder.AppendLine(")</color>");
                }
                else
                {
                    infoBuilder.Append("<color=red>can't hold! (");
                    infoBuilder.Append(currentStr);
                    infoBuilder.Append("/");
                    infoBuilder.Append(reqStr);
                    infoBuilder.AppendLine(")</color>");
                }
                    
            }

            // 청결도 소모 (있는 경우만)
            if (hpCost > 0)
            {
                if (!GameManager.instance.P_isEnglish) infoBuilder.Append($"청결도 소모: <color=orange>-{hpCost}</color>");
                else infoBuilder.Append($"CP decrease: <color=orange>-{hpCost}</color>");
            }

            infoText.text = infoBuilder.ToString();
        }

        // (C) 패널 활성화
        if (uiPanel != null) uiPanel.SetActive(true);
    }

    private void HideInfo()
    {
        // 패널 전체를 끄면 자식 텍스트들도 같이 꺼집니다.
        if (uiPanel != null && uiPanel.activeSelf)
        {
            uiPanel.SetActive(false);
            lastHitTrash = null;
        }
    }
}
