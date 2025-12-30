using UnityEngine;
using UnityEngine.UI;

public class UpgradeSlot : MonoBehaviour
{
    [Header("설정")]
    public UpgradeType type;       // 이 슬롯이 어떤 업그레이드인지
    public string displayName;     // 표시 이름

    [Header("UI 연결")]
    [SerializeField] private Text txtName;      // 이름
    [SerializeField] private Text txtPrice;     // 가격
    [SerializeField] private Slider levelSlider; // 슬라이더
    [SerializeField] private Text txtLevelNum;  // 숫자 (3 / 5)
    [SerializeField] private Button btnBuy;     // 구매 버튼

    private UpgradeManager manager;
    private UpgradeUI parentUI;

    public void Setup(UpgradeManager mgr, UpgradeUI ui)
    {
        manager = mgr;
        parentUI = ui;

        if (txtName != null) txtName.text = displayName;

        if (btnBuy != null)
        {
            btnBuy.onClick.RemoveAllListeners();
            btnBuy.onClick.AddListener(TryBuy);
        }
    }

    public void Refresh()
    {
        if (manager == null) return;

        int cost = manager.GetUpgradeCost(type);
        int currentLv = 0;
        int maxLv = 10;

        // 1. 레벨 계산 (힘은 -1 처리하여 0부터 시작하게 함)
        CalculateLevelInfo(out currentLv, out maxLv);

        // 2. 만렙(MAX) 여부 판단 로직 수정 [핵심 문제 해결 구간]
        bool isMax = false;

        if (type == UpgradeType.PickNPC)
        {
            // [히든 업그레이드] 비용이 0원이라고 MAX가 아님. 실제 레벨이 1이어야 MAX임.
            // (UpgradeManager에서 잠금 상태일 때 비용을 0으로 주기 때문에 분리해야 함)
            isMax = (currentLv >= maxLv);
        }
        else
        {
            // [일반 업그레이드] 레벨이 꽉 찼거나, 더 이상 비용이 없을 때(0원)
            isMax = (currentLv >= maxLv) || (cost == 0);
        }

        // [힘 예외 처리] 히든 스탯(7)에 도달했으면 무조건 MAX 처리 및 슬라이더 고정
        if (type == UpgradeType.Strength && GameManager.instance.P_Str >= UpgradeManager.STR_ULTIMATE)
        {
            isMax = true;
            currentLv = maxLv; // 슬라이더가 뚫리지 않게 꽉 찬 상태로
        }

        // 3. 슬라이더 UI 적용
        if (levelSlider != null)
        {
            levelSlider.minValue = 0;
            levelSlider.maxValue = maxLv;
            levelSlider.value = currentLv;
            levelSlider.wholeNumbers = true;
        }

        // 4. 텍스트 표시 로직 (중간엔 3/5, 끝날 때만 MAX)
        if (txtLevelNum != null)
        {
            if (isMax)
            {
                txtLevelNum.text = "MAX";
                txtLevelNum.color = Color.red;
            }
            else
            {
                // 평소에는 "현재 / 최대" 표시
                txtLevelNum.text = $"{currentLv} / {maxLv}";
                txtLevelNum.color = Color.black;
            }
        }

        // 5. 버튼 및 가격 텍스트 상태 결정
        if (isMax)
        {
            if (txtPrice != null) txtPrice.text = "Complete";
            if (btnBuy != null) btnBuy.interactable = false;
        }
        else
        {
            // 히든 업그레이드(PickNPC)이면서 아직 해금이 안 된 경우 처리
            if (type == UpgradeType.PickNPC && !manager.IsAllStatMaxed())
            {
                if (txtPrice != null) txtPrice.text = "Locked";
                if (btnBuy != null) btnBuy.interactable = false;
            }
            else
            {
                // 일반적인 구매 가능 상태
                if (txtPrice != null) txtPrice.text = $"{cost:N0} G";

                bool canAfford = GameManager.instance.P_Money >= cost;
                if (btnBuy != null) btnBuy.interactable = canAfford;
            }
        }
    }

    private void CalculateLevelInfo(out int cur, out int max)
    {
        cur = 0; max = 1;

        switch (type)
        {
            case UpgradeType.Strength:
                // 시작 힘이 1이어도 UI 상으로는 0(빈 칸)으로 보이게 -1 처리
                cur = GameManager.instance.P_Str - 1;
                max = UpgradeManager.MAX_STR_NORMAL - 1; // 6 - 1 = 5단계
                break;

            case UpgradeType.BagWeight:
                // [수정] 시작값을 10,000으로 고정, 증가폭 8,000
                int baseBag = 10000;
                int stepBag = 8000;
                cur = (GameManager.instance.P_Maxbag - baseBag) / stepBag;
                max = (UpgradeManager.MAX_BAG_WEIGHT - baseBag) / stepBag;
                break;

            case UpgradeType.MaxHP:
                int baseHP = 100;
                cur = (GameManager.instance.P_MaxHP - baseHP) / 80;
                max = (UpgradeManager.MAX_HP_LIMIT - baseHP) / 80;
                break;

            case UpgradeType.PickSpeed:
                float baseSpd = 1.5f;
                float minSpd = UpgradeManager.MIN_PICK_SPEED;
                float totalProgress = baseSpd - minSpd;
                float currentProgress = baseSpd - GameManager.instance.grab_speed;

                max = 5;
                cur = Mathf.RoundToInt((currentProgress / totalProgress) * max);
                break;

            case UpgradeType.MultiGrab:
                // 1개(기본) -> 0레벨로 취급
                cur = GameManager.instance.grab_limit - 1;
                max = UpgradeManager.MAX_GRAB_LIMIT - 1;
                break;

            case UpgradeType.PickNPC:
                // 힘이 궁극(7)이면 획득한 것(1), 아니면 0
                cur = (GameManager.instance.P_Str == UpgradeManager.STR_ULTIMATE) ? 1 : 0;
                max = 1;
                break;
        }

        // 방어 코드: 계산 결과가 음수가 되지 않도록
        if (cur < 0) cur = 0;
        // 방어 코드: 현재 레벨이 최대 레벨을 시각적으로 넘지 않도록 (단, Strength는 Refresh에서 별도 처리)
        if (type != UpgradeType.Strength && cur > max) cur = max;
    }

    private void TryBuy()
    {
        bool success = manager.TryPurchaseUpgrade(type);

        if (success)
        {
            if (AudioManager.instance != null)
            {
                AudioManager.instance.PlaySFX("SFX13");
            }
            parentUI.RefreshAll(true);
        }
    }
}
