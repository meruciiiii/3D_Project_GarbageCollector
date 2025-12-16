using UnityEngine;
using UnityEngine.UI;

public class UpgradeUI : MonoBehaviour
{
    [Header("시스템 연결")]
    [SerializeField] private UpgradeManager upgradeManager; // 인스펙터에서 드래그 앤 드롭

    [Header("UI 연결")]
    [SerializeField] public Text moneyText;       // 현재 돈 표시
    [SerializeField] public Text strPriceText;    // 힘 업그레이드 가격 표시
    [SerializeField] public Text bagPriceText;    // 가방 업그레이드 가격 표시

    [Header("플레이어 연결")]
    // UI가 켜지면 플레이어를 멈추기 위해 필요합니다.
    public FirstPersonMovement playerController;

    private void OnEnable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (playerController != null)
        {
            playerController.enabled = false;
            // PlayerController의 Update들이 멈춰서 시점이 고정됩니다.
        }

        // UI가 켜질 때 텍스트 갱신
        UpdateUI();
    }
    private void OnDisable()
    {
        // 1. 커서 숨기기 & 중앙 고정 (다시 게임 플레이 모드)
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        // 2. 플레이어 움직임/회전 다시 켜기
        if (playerController != null)
        {
            playerController.enabled = true;
        }
    }
    public void OnClick_UpgradeStrength()
    {
        if (upgradeManager.TryPurchaseUpgrade(UpgradeType.Strength))
        {
            UpdateUI(); // 구매 성공 시 UI 갱신 (돈 줄어들고 가격 오름)
            
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
    public void UpdateUI()
    {
        // GameManager가 아직 로딩 안됐으면 무시 (안전장치)
        if (GameManager.instance == null || upgradeManager == null) return;

        // 돈 표시
        moneyText.text = $"보유 자원: {GameManager.instance.P_Money}";

        // 가격 표시 (UpgradeSystem에게 현재 가격 물어보기)
        int strCost = upgradeManager.GetUpgradeCost(UpgradeType.Strength);
        int bagCost = upgradeManager.GetUpgradeCost(UpgradeType.BagWeight);

        strPriceText.text = $"비용: {strCost}";
        bagPriceText.text = $"비용: {bagCost}";
    }
}
