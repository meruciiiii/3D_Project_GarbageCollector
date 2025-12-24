using UnityEngine;

public enum UpgradeType
{
    Strength,   // 힘
    BagWeight,  // 가방 무게
    MaxHP,      // 이동 속도
    PickSpeed, // 줍기 속도
    MultiGrab, // 줍기 개수
    PickNPC // 인간 들기, 힘 상한선 뚫기
}
public class UpgradeManager : MonoBehaviour
{
    [Header("업그레이드 비용 설정 (Final: 15만 이하)")]
    [SerializeField] private int baseStrengthCost = 5000;  // 힘
    [SerializeField] private int baseBagCost = 5000;       // 가방
    [SerializeField] private int baseMaxHPCost = 5000;     // 체력
    [SerializeField] private int baseSpeedCost = 5000;     // 속도
    [SerializeField] private int baseMultiGrabCost = 10000;// 멀티
    [SerializeField] private int pickNpcCost = 150000;     // 히든

    // [최대치 상수]
    public const int MAX_BAG_WEIGHT = 30000;
    public const int MAX_HP_LIMIT = 300;     // [변경] 200 -> 300 (탱커급 체력)
    public const int MAX_STR_NORMAL = 6;
    public const int STR_ULTIMATE = 7;
    public const float MIN_PICK_SPEED = 0.25f;
    public const int MAX_GRAB_LIMIT = 6;

    public bool IsAllStatMaxed()
    {
        if (GameManager.instance == null) return false;

        bool isStrMax = GameManager.instance.P_Str >= MAX_STR_NORMAL;
        bool isBagMax = GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT;
        bool isHpMax = GameManager.instance.P_MaxHP >= MAX_HP_LIMIT;
        bool isSpeedMax = GameManager.instance.grab_speed <= MIN_PICK_SPEED;
        bool isGrabMax = GameManager.instance.grab_limit >= MAX_GRAB_LIMIT;

        return isStrMax && isBagMax && isHpMax && isSpeedMax && isGrabMax;
    }

    public int GetUpgradeCost(UpgradeType type)
    {
        if (GameManager.instance == null) return 0;
        int level = 0;

        switch (type)
        {
            case UpgradeType.Strength:
                if (GameManager.instance.P_Str >= MAX_STR_NORMAL) return 0;
                return baseStrengthCost * (GameManager.instance.P_Str * GameManager.instance.P_Str);

            case UpgradeType.BagWeight:
                if (GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT) return 0;
                level = (GameManager.instance.P_Maxbag - 5000) / 5000;
                return baseBagCost + (level * 25000);

            case UpgradeType.MaxHP:
                if (GameManager.instance.P_MaxHP >= MAX_HP_LIMIT) return 0;

                // [변경] 레벨 계산식: (현재 - 100) / 40
                // 100일 때 0렙, 140일 때 1렙...
                level = (GameManager.instance.P_MaxHP - 100) / 40;

                // 가격: 5000 + (단계 * 20,000) -> 유지
                return baseMaxHPCost + (level * 20000);

            case UpgradeType.PickSpeed:
                if (GameManager.instance.grab_speed <= MIN_PICK_SPEED) return 0;
                float speedDiff = 1.5f - GameManager.instance.grab_speed;
                level = Mathf.RoundToInt(speedDiff * 4);
                return baseSpeedCost + (level * 25000);

            case UpgradeType.MultiGrab:
                if (GameManager.instance.grab_limit >= MAX_GRAB_LIMIT) return 0;
                level = GameManager.instance.grab_limit - 1;
                return baseMultiGrabCost + (level * 35000);

            case UpgradeType.PickNPC:
                if (IsAllStatMaxed() && GameManager.instance.P_Str < STR_ULTIMATE)
                    return pickNpcCost;
                return 0;

            default: return 0;
        }
    }

    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        int cost = GetUpgradeCost(type);
        if (cost == 0 || GameManager.instance.P_Money < cost) return false;

        GameManager.instance.P_Money -= cost;

        switch (type)
        {
            case UpgradeType.Strength:
                GameManager.instance.P_Str += 1;
                break;

            case UpgradeType.BagWeight:
                GameManager.instance.P_Maxbag += 5000;
                break;

            case UpgradeType.MaxHP:
                // [변경] 한 번에 +40씩 시원하게 증가 (5회 마스터)
                GameManager.instance.P_MaxHP += 40;
                GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP;
                break;

            case UpgradeType.PickSpeed:
                GameManager.instance.grab_speed -= 0.25f;
                if (GameManager.instance.grab_speed < MIN_PICK_SPEED)
                    GameManager.instance.grab_speed = MIN_PICK_SPEED;
                break;

            case UpgradeType.MultiGrab:
                GameManager.instance.grab_limit += 1;
                GameManager.instance.grab_range = 1f + ((GameManager.instance.grab_limit - 1) * 0.2f);
                break;

            case UpgradeType.PickNPC:
                GameManager.instance.P_Str = STR_ULTIMATE;
                break;
        }

        GameManager.instance.SaveAllGamedata();
        return true;
    }
}
