using UnityEngine;

public class UpgradeStation : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private UpgradeUI upgradeUI; // 우리가 만든 UI 패널
    [SerializeField] private GameObject interactGuideText; // (선택사항) "F키를 누르세요" 안내 문구

    private bool isPlayerNearby = false;

    private void Start()
    {
        // 시작할 때 안내 문구는 꺼둡니다.
        if (interactGuideText != null) interactGuideText.SetActive(false);
    }
    private void Update()
    {
        // 1. 플레이어가 근처에 있고(isPlayerNearby)
        // 2. 상호작용 키(F)를 눌렀을 때
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            ToggleUI();
        }

        // (참고) 만약 UI가 켜져 있을 때 ESC를 누르면 닫는 기능도 필요하면 여기에 추가합니다.
        if (upgradeUI.gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
        {
            upgradeUI.gameObject.SetActive(false);
        }
    }
    private void ToggleUI()
    {
        bool isActive = upgradeUI.gameObject.activeSelf;

        // 현재 상태의 반대로 설정 (켜져있으면 끄고, 꺼져있으면 켭니다)
        upgradeUI.gameObject.SetActive(!isActive);
    }
    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"인 오브젝트만 인식합니다.
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            Debug.Log("플레이어 접근 확인");

            if (interactGuideText != null) interactGuideText.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;

            // 멀어지면 안내 문구 끄기
            if (interactGuideText != null) interactGuideText.SetActive(false);

            // 멀어지면 상점 UI도 강제로 닫아주는 것이 자연스럽습니다.
            upgradeUI.gameObject.SetActive(false);
        }
    }
}
