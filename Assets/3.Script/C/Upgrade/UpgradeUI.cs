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
    [SerializeField] public Text maxHPPriceText;
    [SerializeField] public Text speedPriceText;
    [SerializeField] public Text multiGrabPriceText;

    [Header("플레이어 연결")]
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

            // 이동 멈출 때 잔여 속도가 남지 않게 하려면 Rigidbody 초기화 등을 고려해야 하지만,
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
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.Strength))
        {
            UpdateUI();
            Debug.Log("힘 업그레이드 성공");
        }
    }

    public void OnClick_BagButton()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.BagWeight))
        {
            UpdateUI();
            Debug.Log("가방 업그레이드 성공");
        }
    }
    public void OnClick_MaxHPButton()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.MaxHP))
        {
            UpdateUI();
            Debug.Log("최대 청결도 업그레이드 성공");
        }
    }
    public void OnClick_UpgradeSpeed()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.PickSpeed))
        {
            UpdateUI();
            Debug.Log("속도 업그레이드 성공");
        }
    }

    public void OnClick_UpgradeMultiGrab()
    {
        // UpgradeType.MultiGrab은 UpgradeManager에서 정의해야 함
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.MultiGrab))
        {
            UpdateUI();
            Debug.Log("흡입구 확장(다중 줍기) 업그레이드 성공");
        }
    }

    public void UpdateUI()
    {
        if (GameManager.instance == null || upgradeManager == null) return;

        moneyText.text = $"보유 자원: {GameManager.instance.P_Money}";

        if (strPriceText != null)
            strPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.Strength)}";

        if (bagPriceText != null)
            bagPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.BagWeight)}";

        if (maxHPPriceText != null)
            maxHPPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.MaxHP)}";

        if (speedPriceText != null)
        {
            // 속도가 거의 최대치(0.25 이하)라면 MAX 표시
            if (GameManager.instance.grab_speed <= 0.25f)
            {
                speedPriceText.text = "MAX LEVEL";
            }
            else
            {
                speedPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.PickSpeed)}";
            }
        }

        if (multiGrabPriceText != null)
        {
            // 최대 개수 제한 (예: 5개) 도달 시 MAX 표시
            // UpgradeManager에서 설정한 제한(5)과 맞춰주세요.
            if (GameManager.instance.grab_limit >= 5)
            {
                multiGrabPriceText.text = "MAX LEVEL";
            }
            else
            {
                multiGrabPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.MultiGrab)}";
            }
        }
    }
}


