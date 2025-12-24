using UnityEngine;

public class SellStation : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] public SellUI sellUI;
    [SerializeField] private GameObject guideText; // "F키를 눌러 상점 열기" 텍스트 오브젝트

    private bool isPlayerNearby = false;
    private PlayerInput targetInput;

    private void Start()
    {
        if (guideText != null) guideText.SetActive(false);
    }
    private void Update()
    {
        // UI(상점/정산창)가 켜져 있으면 안내 문구를 강제로 끕니다.
        if (sellUI != null && sellUI.gameObject.activeSelf)
        {
            if (guideText != null) guideText.SetActive(false);
        }
        else if (isPlayerNearby) // UI가 꺼져있고 플레이어가 근처에 있으면 켭니다.
        {
            if (guideText != null) guideText.SetActive(true);
        }
    }

    private void TryOpenShop()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
        {
            return;
        }

        if (sellUI != null) sellUI.OpenSellMenu();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // 1. 안내 문구 켜기
            if (guideText != null) guideText.SetActive(true);

            // 2. 플레이어의 입력 스크립트(PlayerInput) 가져오기
            targetInput = other.GetComponent<PlayerInput>();

            // 3. 이벤트 구독 (이제 F키 누르면 TryOpenShop이 실행됨)
            if (targetInput != null)
            {
                targetInput.onInteract += TryOpenShop;
            }
        }
    }

    // Trigger 영역 이탈 감지
    private void OnTriggerExit(Collider other)
    {
        // 안내 문구 끄기
        if (guideText != null) guideText.SetActive(false);

        // 이벤트 구독 해제 (안 하면 멀리서도 상점이 열림)
        if (targetInput != null)
        {
            targetInput.onInteract -= TryOpenShop;
            targetInput = null; // 참조 비우기
        }

        // 3. 상점 UI 강제 종료
        if (sellUI != null) sellUI.CloseAllPanels();
    }
    private void OnDisable()
    {
        if (targetInput != null)
        {
            targetInput.onInteract -= TryOpenShop;
            targetInput = null;
        }
    }
}
