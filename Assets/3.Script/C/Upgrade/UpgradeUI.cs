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

        moneyText.text = $"{GameManager.instance.P_Money}";

        // 1. 힘 (상수 참조)
        if (strPriceText != null)
        {
            if (GameManager.instance.P_Str >= UpgradeManager.MAX_STR_NORMAL)
            {
                if (GameManager.instance.P_Str >= UpgradeManager.STR_ULTIMATE)
                    strPriceText.text = "ULTIMATE MAX";
                else
                    strPriceText.text = "MAX";
            }
            else
            {
                strPriceText.text = $"Price: {upgradeManager.GetUpgradeCost(UpgradeType.Strength)}";
            }
        }

        // 2. 가방 (여기가 문제였음! 5100 -> 상수(100,000)로 변경)
        if (bagPriceText != null)
        {
            // UpgradeManager에 있는 100,000 값을 가져와서 비교합니다.
            if (GameManager.instance.P_Maxbag >= UpgradeManager.MAX_BAG_WEIGHT)
            {
                bagPriceText.text = "MAX LEVEL";
            }
            else
            {
                bagPriceText.text = $"Price: {upgradeManager.GetUpgradeCost(UpgradeType.BagWeight)}";
            }
        }

        // 3. 체력
        if (maxHPPriceText != null)
        {
            if (GameManager.instance.P_MaxHP >= UpgradeManager.MAX_HP_LIMIT)
                maxHPPriceText.text = "MAX LEVEL";
            else
                maxHPPriceText.text = $"Price: {upgradeManager.GetUpgradeCost(UpgradeType.MaxHP)}";
        }

        // 4. 속도
        if (speedPriceText != null)
        {
            if (GameManager.instance.grab_speed <= UpgradeManager.MIN_PICK_SPEED)
                speedPriceText.text = "MAX LEVEL";
            else
                speedPriceText.text = $"Price: {upgradeManager.GetUpgradeCost(UpgradeType.PickSpeed)}";
        }

        // 5. 다중 줍기
        if (multiGrabPriceText != null)
        {
            if (GameManager.instance.grab_limit >= UpgradeManager.MAX_GRAB_LIMIT)
                multiGrabPriceText.text = "MAX LEVEL";
            else
                multiGrabPriceText.text = $"Price: {upgradeManager.GetUpgradeCost(UpgradeType.MultiGrab)}";
        }

        // ... (히든 업그레이드 UI 로직 유지) ...
        if (btnPickNPC != null && txtPickNPCInfo != null)
        {
            if (upgradeManager.IsAllStatMaxed())
            {
                if (GameManager.instance.P_Str >= UpgradeManager.STR_ULTIMATE)
                {
                    txtPickNPCInfo.text = "Purchase completed (힘 7)";
                    btnPickNPC.interactable = false;
                }
                else
                {
                    int cost = upgradeManager.GetUpgradeCost(UpgradeType.PickNPC);
                    txtPickNPCInfo.text = $"PickUP Human (한계돌파)\nPrice: {cost}";
                    btnPickNPC.interactable = true;
                }
            }
            else
            {
                txtPickNPCInfo.text = "???";
                btnPickNPC.interactable = false;
            }
        }
    }
}


