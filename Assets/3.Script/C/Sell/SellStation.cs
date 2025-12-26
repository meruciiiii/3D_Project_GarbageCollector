using UnityEngine;

public class SellStation : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] public SellUI sellUI;
    [SerializeField] private GameObject guideText; // "F키를 눌러 상점 열기" 텍스트 오브젝트

    private bool isPlayerNearby = false;
    private PlayerInput targetInput;

    private void Update()
    {
        // 매 프레임 상황을 체크해서 텍스트를 켤지 끌지 결정합니다.

        // 1. UI가 켜져 있다면? -> 안내 문구 방해되니까 무조건 끔
        if (sellUI != null && sellUI.IsUIActive)
        {
            if (guideText != null) guideText.SetActive(false);
        }
        // 2. UI는 꺼져있는데, 플레이어가 근처에 있다면? -> 안내 문구 켬!
        else if (isPlayerNearby)
        {
            if (guideText != null) guideText.SetActive(true);
        }
        // 3. 그 외 (플레이어가 멀리 있음) -> 끔
        else
        {
            if (guideText != null) guideText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // [핵심] 플레이어 입장 확인
            isPlayerNearby = true;

            // 입력 스크립트 연결
            targetInput = other.GetComponent<PlayerInput>();
            if (targetInput != null)
            {
                targetInput.onInteract += TryOpenShop;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // [핵심] 플레이어 퇴장 확인
            isPlayerNearby = false;

            // 입력 연결 해제
            if (targetInput != null)
            {
                targetInput.onInteract -= TryOpenShop;
                targetInput = null;
            }
        }
    }

    private void TryOpenShop()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;
        if (sellUI != null) sellUI.OpenSellMenu();
    }
}
