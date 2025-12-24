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
        Ray ray = new Ray(mainCamera.transform.position, mainCamera.transform.forward);
        RaycastHit hit;

        // 1. 레이캐스트가 무언가에 맞았고 + 그게 쓰레기 레이어인가?
        if (Physics.Raycast(ray, out hit, checkDistance, trashLayerMask))
        {
            // 2. 그 물체에 Trash 컴포넌트가 있는가?
            if (hit.collider.TryGetComponent<Trash>(out Trash trash))
            {
                // 최적화: 이미 같은 대상을 보고 있고 + 그 대상이 파괴되지 않았다면 리턴
                if (currentTarget == hit.transform) return;

                // 새로운 쓰레기를 봄 -> 타겟 갱신 및 UI 표시
                currentTarget = hit.transform;
                ShowInfo(trash, hit.collider.gameObject);

                // [중요] 쓰레기를 찾았으면 여기서 함수를 끝냅니다. 
                // 아래의 HideTooltip()이 실행되지 않도록!
                return;
            }
        }
        HideTooltip();
        currentTarget = null;
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
                tooltipText.text = $"{tName}\n<color=yellow>{tWeight} G</color>";
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
