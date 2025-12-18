using UnityEngine;
using UnityEngine.UI;


public class UpgradeStation : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private UpgradeUI upgradeUI; // 우리가 만든 UI 패널
    [SerializeField] private GameObject interactGuideText; // (선택사항) "F키를 누르세요" 안내 문구

    private PlayerInput targetInput;

    private void Start()
    {
        // 시작할 때 안내 문구는 꺼둡니다.
        if (interactGuideText != null) interactGuideText.SetActive(false);
    }

    // 이벤트에 연결될 함수 (UI 켜고 끄기)
    private void ToggleUI()
    {
        if (upgradeUI == null) return;

        bool isActive = upgradeUI.gameObject.activeSelf;
        // 현재 상태의 반대로 설정 (켜져있으면 끄고, 꺼져있으면 켭니다)
        upgradeUI.gameObject.SetActive(!isActive);
    }

    private void OnTriggerEnter(Collider other)
    {
        // 태그가 "Player"인 오브젝트만 인식합니다.
        if (other.CompareTag("Player"))
        {
            // [수정된 부분] 플레이어에게서 PlayerInput 컴포넌트를 가져와서 변수에 담아야 합니다!
            targetInput = other.GetComponent<PlayerInput>();

            // 가져오는 데 성공했다면 이벤트 구독
            if (targetInput != null)
            {
                targetInput.onInteract += ToggleUI;
            }

            if (interactGuideText != null) interactGuideText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 플레이어가 나갈 때 이벤트 구독 해제 (중요)
            if (targetInput != null)
            {
                targetInput.onInteract -= ToggleUI;
                targetInput = null; // 참조 비우기
            }

            // 멀어지면 안내 문구 끄기
            if (interactGuideText != null) interactGuideText.SetActive(false);

            // 멀어지면 상점 UI도 강제로 닫아주는 것이 자연스럽습니다.
            if (upgradeUI != null) upgradeUI.gameObject.SetActive(false);
        }
    }
}
