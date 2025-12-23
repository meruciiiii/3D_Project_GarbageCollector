using UnityEngine;
using UnityEngine.UI;

public class ThashInfo : MonoBehaviour
{
    [Header("UI 설정")]
    [SerializeField] private Text tooltipText; // 정보를 띄울 텍스트
    [SerializeField] private GameObject tooltipPanel;

    [Header("감지 설정")]
    [SerializeField] private float checkDistance = 3.0f; // 감지 거리 (PlayerWork와 비슷하게)
    [SerializeField] private LayerMask trashLayerMask; // 쓰레기 레이어만 감지

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
        // 화면 정중앙으로 레이를 쏨
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        // 레이캐스트 충돌 감지
        if (Physics.Raycast(ray, out hit, checkDistance, trashLayerMask))
        {
            // 최적화: 같은 대상을 계속 보고 있다면 UI 갱신 안 함
            if (currentTarget == hit.transform) return;

            // 쓰레기 컴포넌트가 있는지 확인
            if (hit.collider.TryGetComponent<Trash>(out Trash trash))
            {
                currentTarget = hit.transform;
                ShowInfo(trash, hit.collider.gameObject);
                return;
            }
        }

        // 아무것도 없거나 쓰레기가 아니면 UI 끄기
        if (currentTarget != null)
        {
            currentTarget = null;
            HideTooltip();
        }
    }

    private void ShowInfo(Trash trash, GameObject obj)
    {
        // 1. 키 생성 로직 (CSV 데이터베이스 접근용)
        int trashNum = trash.getTrashNum();
        string layerName = LayerMask.LayerToName(obj.layer);

        // 레이어 이름에 따라 접두사 결정 (기존 로직 준수)
        // 유니티 에디터의 레이어 이름이 정확해야 합니다.
        string keyPrefix = (layerName == "BigTrash") ? "large_" : "small_";
        string key = keyPrefix + trashNum;

        // 2. 데이터베이스에서 정보 가져오기
        if (CSV_Database.instance != null)
        {
            string tName = CSV_Database.instance.getname(key);
            int tWeight = CSV_Database.instance.getweight(key);

            // 데이터가 비어있으면 키값이라도 출력 (디버깅용)
            if (string.IsNullOrEmpty(tName)) tName = "Unknown";

            // 3. UI 갱신
            if (tooltipText != null)
            {
                tooltipText.text = $"{tName}\n<color=yellow>{tWeight} kg</color>";
            }

            if (tooltipPanel != null) tooltipPanel.SetActive(true);
            if (tooltipText != null) tooltipText.gameObject.SetActive(true);
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null) tooltipPanel.SetActive(false);
        if (tooltipText != null) tooltipText.gameObject.SetActive(false);
    }
}
