using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("시스템 연결")]
    [SerializeField] private UpgradeManager upgradeManager;

    [Header("UI 연결")]
    [SerializeField] public Text moneyText;
    [SerializeField] public Text strPriceText;
    [SerializeField] public Text bagPriceText;

    [Header("플레이어 연결")]
    // MonoBehaviour 대신 실제 클래스 이름(FirstPersonMovement)을 쓰는 것이 좋습니다.
    public PlayerController playerController;

    private void Awake()
    {
        // 안전장치: 플레이어가 연결 안 되어 있으면 자동으로 찾기
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        // UI가 켜질 때: 커서 보이기 + 조작 정지
        SetPlayerState(false);
        UpdateUI();
    }

    private void OnDisable()
    {
        // UI가 꺼질 때: 커서 숨기기 + 조작 재개
        SetPlayerState(true);
    }
   
    private void SetPlayerState(bool isGameActive)
    {
        // 1. 플레이어 이동/회전 스크립트 끄기/켜기
        if (playerController != null)
        {
            if (!isGameActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero; // Unity 6 (구버전은 velocity)
                    rb.angularVelocity = Vector3.zero;
                }
            }

            playerController.enabled = isGameActive;

            // [중요] 이동 멈출 때 잔여 속도가 남지 않게 하려면 Rigidbody 초기화 등을 고려해야 하지만,
            // 보통 스크립트를 끄면 Update가 멈춰서 시선과 이동이 멈춥니다.
        }

        // 2. 마우스 커서 상태 설정
        if (isGameActive)
        {
            // 게임 중: 커서 숨기고 중앙 고정
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // UI 사용 중: 커서 보이고 자유 이동
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // ... (나머지 버튼 함수들은 기존 유지) ...
    public void OnClick_UpgradeStrength()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.Strength)) UpdateUI();
    }

    public void OnClick_BagButton()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.BagWeight))
        {
            UpdateUI();
            Debug.Log("가방 업그레이드 성공");
        }
    }

    public void UpdateUI()
    {
        if (GameManager.instance == null || upgradeManager == null) return;

        moneyText.text = $"보유 자원: {GameManager.instance.P_Money}";
        strPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.Strength)}";
        bagPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.BagWeight)}";
    }
}


