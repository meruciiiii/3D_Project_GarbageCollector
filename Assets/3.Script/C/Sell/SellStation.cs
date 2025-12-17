using UnityEngine;

public class SellStation : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private SellUI sellUI;
    [SerializeField] private GameObject guideText; // "F키를 눌러 상점 열기" 텍스트 오브젝트

    private bool isPlayerNearby = false;

    private void Start()
    {
        if (guideText != null) guideText.SetActive(false);
    }

    private void Update()
    {
        // 플레이어가 범위 안에 있고 F를 누르면 UI 오픈
        if (isPlayerNearby && Input.GetKeyDown(KeyCode.F))
        {
            if (sellUI != null) sellUI.OpenSellMenu();
        }
    }

    // Trigger 영역 진입 감지
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            if (guideText != null) guideText.SetActive(true);
        }
    }

    // Trigger 영역 이탈 감지
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (guideText != null) guideText.SetActive(false);

            // 멀어지면 상점 UI 강제 종료 (안전장치)
            if (sellUI != null) sellUI.CloseAllPanels();
        }
    }
}
