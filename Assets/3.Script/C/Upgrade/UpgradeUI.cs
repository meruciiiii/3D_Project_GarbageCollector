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

    // [NEW] 히든 업그레이드 UI 연결
    [Header("히든 업그레이드 (인간 들기)")]
    [SerializeField] private Button btnPickNPC;     // 인간 들기 버튼
    [SerializeField] private Text txtPickNPCInfo;   // 가격 및 정보 텍스트

    [Header("플레이어 연결")]
    public PlayerController playerController;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();
    }

    private void OnEnable()
    {
        SetPlayerState(false);
        UpdateUI();
    }

    private void OnDisable()
    {
        SetPlayerState(true);
    }

    private void SetPlayerState(bool isGameActive)
    {
        if (playerController != null)
        {
            if (!isGameActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            playerController.enabled = isGameActive;
        }

        if (isGameActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }

    // --- 기존 버튼 함수들 ---
    public void OnClick_UpgradeStrength()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.Strength)) UpdateUI();
    }

    public void OnClick_BagButton()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.BagWeight)) UpdateUI();
    }
    public void OnClick_MaxHPButton()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.MaxHP)) UpdateUI();
    }
    public void OnClick_UpgradeSpeed()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.PickSpeed)) UpdateUI();
    }

    public void OnClick_UpgradeMultiGrab()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.MultiGrab)) UpdateUI();
    }

    // [NEW] 히든 업그레이드 버튼 함수
    public void OnClick_PickNPC()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.PickNPC))
        {
            UpdateUI(); // 성공 시 UI 갱신 (버튼 잠그기 등)
        }
    }

    public void UpdateUI()
    {
        if (GameManager.instance == null || upgradeManager == null) return;

        moneyText.text = $"보유 자원: {GameManager.instance.P_Money}";

        // 1. 힘 (Strength) - 6 도달 시 MAX 표시
        if (strPriceText != null)
        {
            if (GameManager.instance.P_Str >= 6)
            {
                // 이미 히든까지 뚫었다면 ULTIMATE
                if (GameManager.instance.P_Str >= 7)
                    strPriceText.text = "ULTIMATE MAX";
                else
                    strPriceText.text = "MAX (한계 도달)";
            }
            else
            {
                strPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.Strength)}";
            }
        }

        // 2. 가방 무게 (5100 도달 시 MAX)
        if (bagPriceText != null)
        {
            // UpgradeManager의 MAX_BAG_WEIGHT 상수값(5100)과 맞춰야 함
            if (GameManager.instance.P_Maxbag >= 5100)
                bagPriceText.text = "MAX LEVEL";
            else
                bagPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.BagWeight)}";
        }

        // 3. 최대 청결도 (200 도달 시 MAX)
        if (maxHPPriceText != null)
        {
            if (GameManager.instance.P_MaxHP >= 200)
                maxHPPriceText.text = "MAX LEVEL";
            else
                maxHPPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.MaxHP)}";
        }

        // 4. 줍기 속도
        if (speedPriceText != null)
        {
            if (GameManager.instance.grab_speed <= 0.25f)
                speedPriceText.text = "MAX LEVEL";
            else
                speedPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.PickSpeed)}";
        }

        // 5. 다중 줍기
        if (multiGrabPriceText != null)
        {
            if (GameManager.instance.grab_limit >= 5)
                multiGrabPriceText.text = "MAX LEVEL";
            else
                multiGrabPriceText.text = $"비용: {upgradeManager.GetUpgradeCost(UpgradeType.MultiGrab)}";
        }

        // [NEW] 6. 히든 업그레이드 (인간 들기) 버튼 제어
        if (btnPickNPC != null && txtPickNPCInfo != null)
        {
            // UpgradeManager에 있는 '만렙 판독기' 함수 호출
            if (upgradeManager.IsAllStatMaxed())
            {
                // 자격 조건 달성!
                if (GameManager.instance.P_Str >= 7)
                {
                    // 이미 구매함
                    txtPickNPCInfo.text = "구매 완료 (힘 7)";
                    btnPickNPC.interactable = false; // 버튼 잠금
                }
                else
                {
                    // 구매 가능 상태 (아직 안 샀음)
                    int cost = upgradeManager.GetUpgradeCost(UpgradeType.PickNPC);
                    txtPickNPCInfo.text = $"인간 들기 (한계돌파)\n비용: {cost}";
                    btnPickNPC.interactable = true; // 버튼 활성화
                }
            }
            else
            {
                // 자격 미달 (아직 다른 스탯 만렙 아님)
                txtPickNPCInfo.text = "???\n(모든 능력 MAX 필요)";
                btnPickNPC.interactable = false; // 버튼 잠금
            }
        }
    }
}


