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
        // 2. UI는 꺼져있는데, 플레이어가 근처에 있다면? -> 안내 문구 켬
        else if (isPlayerNearby)
        {
            if (guideText != null) guideText.SetActive(true);
        }
        else
        {
            if (guideText != null) guideText.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            targetInput = other.GetComponent<PlayerInput>();
            if (targetInput != null)
            {
                targetInput.onInteract += ToggleShop; // 이름 변경: TryOpen -> Toggle
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (targetInput != null)
            {
                targetInput.onInteract -= ToggleShop;
                targetInput = null;
            }
            // 멀어지면 강제로 닫기 (안전장치)
            if (sellUI != null && sellUI.IsUIActive)
            {
                sellUI.CloseAllPanels();
            }
        }
    }

    private void ToggleShop()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused) return;

        if (sellUI != null)
        {
            if (sellUI.IsUIActive)
            {
                // 이미 켜져 있으면 -> 닫기
                sellUI.CloseAllPanels();
            }
            else
            {
                // 꺼져 있으면 -> 열기
                sellUI.OpenTradePanel();
            }
        }
    }
}
