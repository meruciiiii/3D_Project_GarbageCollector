using UnityEngine;
using UnityEngine.UI;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Text tooltipText; // 정보를 띄울 텍스트
    [SerializeField] private GameObject tooltipPanel; // 정보를 띄울 패널 (배경)

    [Header("감지 설정")]
    [SerializeField] private float checkDistance = 3.5f; // 감지 거리
    [SerializeField] private LayerMask trashLayerMask; // 쓰레기 레이어 (꼭 설정하세요!)

    private Camera mainCamera;
    private Transform currentTarget;

    private void Start()
    {
        mainCamera = Camera.main;
        HideTooltip(); // 시작할 땐 끔
    }

    private void Update()
    {
        CheckObjectInFront();
    }

    private void CheckObjectInFront()
    {
        // 카메라 정면으로 레이 발사
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        // 1. 레이캐스트 충돌 감지
        if (Physics.Raycast(ray, out hit, checkDistance, trashLayerMask))
        {
            // 2. Trash 컴포넌트가 있는지 확인
            if (hit.collider.TryGetComponent<Trash>(out Trash trash))
            {
                // 최적화: 이미 같은 쓰레기를 보고 있다면 UI 갱신 안 함
                if (currentTarget == hit.transform) return;

                currentTarget = hit.transform;
                ShowInfo(trash);
                return; // 찾았으면 여기서 함수 종료 (HideTooltip 실행 안 함)
            }
        }

        // 아무것도 안 보고 있거나, 쓰레기가 아니면 툴팁 숨김
        HideTooltip();
        currentTarget = null;
    }

    private void ShowInfo(Trash trash)
    {
        // [수정 포인트 1] 레이어 이름 대신, Trash 스크립트의 Enum(Size)을 사용해 정확도 향상
        string keyPrefix = "small_"; // 기본값

        if (trash.Size == Trash.TrashSize.Large)
        {
            keyPrefix = "large_";
        }
        else if (trash.Size == Trash.TrashSize.Human)
        {
            // 나중에 인간 들기 기능이 생기면 여기에 "human_" 같은 키 추가 가능
            keyPrefix = "human_";
        }

        // [수정 포인트 2] 프로퍼티(TrashNum) 사용
        string key = keyPrefix + trash.TrashNum;

        // 3. CSV 데이터베이스에서 정보 가져오기
        if (CSV_Database.instance != null)
        {
            string tName = CSV_Database.instance.getname(key);
            int tWeight = CSV_Database.instance.getweight(key);

            // 데이터가 없으면 디버깅용 텍스트 출력
            if (string.IsNullOrEmpty(tName)) tName = "알 수 없음";

            // 4. UI 갱신 및 패널 켜기
            if (tooltipText != null)
            {
                tooltipText.text = $"{tName}\n<color=yellow>{tWeight} g</color>";
            }

            if (tooltipPanel != null)
            {
                tooltipPanel.SetActive(true);
            }
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            tooltipPanel.SetActive(false);
        }
    }
}
