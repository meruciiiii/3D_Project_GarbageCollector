using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Collections;
using System.Collections.Generic;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Text tooltipText; // 정보를 표시할 UI 텍스트

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
        if (tooltipText != null && tooltipText.gameObject.activeSelf && lastHitTrash == trash) return;

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

        // 3. CSV 데이터 조회
        string tName = CSV_Database.instance.getname(finalKey);
        int tWeight = CSV_Database.instance.getweight(finalKey);
        int reqStr = CSV_Database.instance.getrequireStr(finalKey);     // 요구 힘
        int hpCost = CSV_Database.instance.getHpdecrease(finalKey);     // 청결도 감소치

        // 4. 플레이어 현재 스탯 조회
        int currentStr = GameManager.instance.P_Str; // GameManager에서 현재 힘 가져오기

        // 5. UI 텍스트 조합 (StringBuilder 사용)
        if (tooltipText != null)
        {
            infoBuilder.Clear(); // 기존 내용 초기화

            // (1) 이름
            infoBuilder.AppendLine($"<b>{tName}</b>");

            // (2) 무게
            infoBuilder.AppendLine($"무게: {tWeight} g");

            // (3) 힘 요구량 비교 및 색상 처리
            if (currentStr >= reqStr)
            {
                // 힘이 충분함: 흰색(기본) 표시
                // 예: "필요 힘: 5"
                infoBuilder.AppendLine($"필요 힘: {reqStr}");
            }
            else
            {
                // 힘이 부족함: 빨간색 경고 및 (현재/필요) 표시
                // 예: "힘 부족! (1/5)"
                infoBuilder.Append("<color=red>힘 부족! (");
                infoBuilder.Append(currentStr);
                infoBuilder.Append("/");
                infoBuilder.Append(reqStr);
                infoBuilder.AppendLine(")</color>");
            }

            // (4) 청결도 소모 (값이 있을 때만 표시)
            if (hpCost > 0)
            {
                // 주황색 등으로 경고
                infoBuilder.Append($"청결도 소모: <color=orange>-{hpCost}</color>");
            }

            // 최종 적용 및 활성화
            tooltipText.text = infoBuilder.ToString();
            tooltipText.gameObject.SetActive(true);
        }
    }

    private void HideInfo()
    {
        if (tooltipText != null && tooltipText.gameObject.activeSelf)
        {
            tooltipText.gameObject.SetActive(false);
            lastHitTrash = null;
        }
    }
}
