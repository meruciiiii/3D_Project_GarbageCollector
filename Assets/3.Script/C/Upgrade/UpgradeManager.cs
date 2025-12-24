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
    [Header("업그레이드 가격 설정 (고물가 반영)")]
    // 1회 왕복 수익이 약 50,000원 이라고 가정했을 때의 가격 책정
    [SerializeField] private int baseStrengthCost = 150000; // 힘: 3번 왕복해야 1업
    [SerializeField] private int baseBagCost = 30000;       // 가방: 1번 왕복하면 구매 가능 (3만 -> 4만 -> 5만...)
    [SerializeField] private int baseMaxHPCost = 20000;
    [SerializeField] private int baseSpeedCost = 50000;
    [SerializeField] private int baseMultiGrabCost = 300000; // 아주 비쌈
    [SerializeField] private int pickNpcCost = 2000000;      // 엔딩급 가격

    // [최대치 상수 정의] - 이 숫자를 기억하세요!
    public const int MAX_BAG_WEIGHT = 100000; // 10만까지 업그레이드 가능
    public const int MAX_STR_NORMAL = 6;
    public const int STR_ULTIMATE = 7;
    public const int MAX_HP_LIMIT = 200;
    public const float MIN_PICK_SPEED = 0.25f;
    public const int MAX_GRAB_LIMIT = 5;

    // 만렙 체크 함수
    public bool IsAllStatMaxed()
    {
        if (GameManager.instance == null) return false;

        bool isStrMax = GameManager.instance.P_Str >= MAX_STR_NORMAL;
        bool isBagMax = GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT; // 10만 기준
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
                // 힘 제곱 비례: 15만, 60만, 135만...
                return baseStrengthCost * (GameManager.instance.P_Str * GameManager.instance.P_Str);

            case UpgradeType.BagWeight:
                if (GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT) return 0;

                // [수정된 공식]
                // 현재 가방 10000 시작. 5000 단위로 업그레이드.
                // 레벨 = (현재 - 10000) / 5000  => 0, 1, 2, 3...
                level = (GameManager.instance.P_Maxbag - 10000) / 5000;

                // 가격: 기본 3만 + (레벨 * 1만) => 3만, 4만, 5만... 점진적 증가
                return baseBagCost + (level * 10000);

            case UpgradeType.MaxHP:
                if (GameManager.instance.P_MaxHP >= MAX_HP_LIMIT) return 0;
                level = (GameManager.instance.P_MaxHP - 100) / 10;
                return baseMaxHPCost + (level * 10000);

            case UpgradeType.PickSpeed:
                if (GameManager.instance.grab_speed <= MIN_PICK_SPEED) return 0;
                float speedDiff = 1.5f - GameManager.instance.grab_speed;
                level = Mathf.RoundToInt(speedDiff * 10);
                return baseSpeedCost + (level * 20000);

            case UpgradeType.MultiGrab:
                if (GameManager.instance.grab_limit >= MAX_GRAB_LIMIT) return 0;
                return baseMultiGrabCost * GameManager.instance.grab_limit;

            case UpgradeType.PickNPC:
                if (IsAllStatMaxed() && GameManager.instance.P_Str < STR_ULTIMATE)
                    return pickNpcCost;
                return 0;

            default: return 0;
        }
    }

    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        // 비용 체크
        int cost = GetUpgradeCost(type);
        if (cost == 0 || GameManager.instance.P_Money < cost) return false;

        // 결제
        GameManager.instance.P_Money -= cost;

        // 적용
        switch (type)
        {
            case UpgradeType.Strength:
                GameManager.instance.P_Str += 1;
                break;

            case UpgradeType.BagWeight:
                // [중요] 한 번에 5000씩 증가 (단위가 크므로)
                GameManager.instance.P_Maxbag += 5000;
                break;

            case UpgradeType.MaxHP:
                GameManager.instance.P_MaxHP += 10;
                GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP;
                break;

            case UpgradeType.PickSpeed:
                GameManager.instance.grab_speed -= 0.1f;
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
