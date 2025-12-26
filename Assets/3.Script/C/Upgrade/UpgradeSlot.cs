using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [Header("설정")]
    public UpgradeType type;       // 이 슬롯이 어떤 업그레이드인지 (Strength, BagWeight 등)
    public string displayName;     // 표시 이름 (예: 근력 강화)

    [Header("UI 연결")]
    [SerializeField] private Text txtName;      // 이름 텍스트
    [SerializeField] private Text txtPrice;     // 가격 텍스트
    [SerializeField] private Slider levelSlider; // 단계를 보여줄 슬라이더
    [SerializeField] private Text txtLevelNum;  // (선택) "3 / 5" 숫자 표시용
    [SerializeField] private Button btnBuy;     // 구매 버튼

    private UpgradeManager manager;
    private UpgradeUI parentUI;

    // 초기화
    public void Setup(UpgradeManager mgr, UpgradeUI ui)
    {
        manager = mgr;
        parentUI = ui;

        if (txtName != null) txtName.text = displayName;

        // 버튼 클릭 시 구매 시도
        if (btnBuy != null)
        {
            btnBuy.onClick.RemoveAllListeners();
            btnBuy.onClick.AddListener(TryBuy);
        }
    }

    // 정보 갱신 (슬라이더 및 가격 업데이트)
    public void Refresh()
    {
        if (manager == null) return;

        int cost = manager.GetUpgradeCost(type);
        int currentLv = 0;
        int maxLv = 10; // 기본값

        // 1. 타입별로 현재 레벨과 만렙 계산 (슬라이더용)
        CalculateLevelInfo(out currentLv, out maxLv);

        // 2. 슬라이더 설정
        if (levelSlider != null)
        {
            levelSlider.minValue = 0;
            levelSlider.maxValue = maxLv;
            levelSlider.value = currentLv;
            levelSlider.wholeNumbers = true; // 정수 단위로 딱딱 끊어지게
        }

        // 3. 레벨 텍스트 (예: 3 / 7)
        if (txtLevelNum != null)
        {
            txtLevelNum.text = $"{currentLv} / {maxLv}";
        }

        // 4. 가격 및 버튼 상태
        bool isMax = (currentLv >= maxLv);

        if (isMax)
        {
            if (txtPrice != null) txtPrice.text = "MAX";
            if (btnBuy != null) btnBuy.interactable = false;
        }
        else
        {
            if (txtPrice != null) txtPrice.text = $"{cost} G";

            // 돈 부족하면 버튼 비활성화
            bool canAfford = GameManager.instance.P_Money >= cost;
            if (btnBuy != null) btnBuy.interactable = canAfford;
        }

        // 히든(PickNPC) 특별 처리: 힘이 부족하면 잠금
        if (type == UpgradeType.PickNPC)
        {
            if (GameManager.instance.P_Str < UpgradeManager.STR_ULTIMATE)
            {
                if (btnBuy != null) btnBuy.interactable = false;
                if (txtPrice != null) txtPrice.text = "힘 부족 (Lv.7 필요)";
            }
        }
    }

    // 실제 스탯을 "레벨(숫자)"로 변환하는 계산기
    private void CalculateLevelInfo(out int cur, out int max)
    {
        cur = 0; max = 1; // 기본값

        switch (type)
        {
            case UpgradeType.Strength:
                cur = GameManager.instance.P_Str;
                max = UpgradeManager.STR_ULTIMATE; // 7
                break;

            case UpgradeType.BagWeight:
                // 기본 10000, +5000씩 증가한다고 가정 시
                int baseBag = 10000;
                cur = (GameManager.instance.P_Maxbag - baseBag) / 5000;
                max = (UpgradeManager.MAX_BAG_WEIGHT - baseBag) / 5000;
                break;

            case UpgradeType.MaxHP:
                // 기본 100, +40씩 증가
                int baseHP = 100;
                cur = (GameManager.instance.P_MaxHP - baseHP) / 40;
                max = (UpgradeManager.MAX_HP_LIMIT - baseHP) / 40;
                break;

            case UpgradeType.PickSpeed: // 속도는 값이 줄어듦 (1.5 -> 0.25)
                float baseSpd = 1.5f;
                float minSpd = UpgradeManager.MIN_PICK_SPEED;
                // 역순 계산 (작을수록 고렙)
                float totalProgress = baseSpd - minSpd;
                float currentProgress = baseSpd - GameManager.instance.grab_speed;

                // 0~5단계로 환산
                max = 5;
                cur = Mathf.RoundToInt((currentProgress / totalProgress) * max);
                break;

            case UpgradeType.MultiGrab:
                cur = GameManager.instance.grab_limit - 1; // 기본 1개부터 시작
                max = UpgradeManager.MAX_GRAB_LIMIT - 1;
                break;

            case UpgradeType.PickNPC:
                cur = manager.IsAllStatMaxed() ? 1 : 0; // 0 or 1 (단일 업글)
                max = 1;
                break;
        }
    }

    private void TryBuy()
    {
        bool success = manager.TryPurchaseUpgrade(type);
        if (success)
        {
            parentUI.RefreshAll(); // 전체 새로고침 (돈이 줄었으니까)
        }
    }
}
