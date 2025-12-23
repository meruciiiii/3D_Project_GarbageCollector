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
    [Header("업그레이드 가격 설정")]
    [SerializeField] private int baseStrengthCost = 1000;
    [SerializeField] private int baseBagCost = 500;
    [SerializeField] private int baseMaxHPCost = 1500;
    [SerializeField] private int baseSpeedCost = 2000;
    [SerializeField] private int baseMultiGrabCost = 3000;
    [SerializeField] private int pickNpcCost = 50000; // [NEW] 인간 들기 비용 (최종 콘텐츠답게 비싸게 설정)

    // [NEW] 각 능력치의 '최대 한계(졸업)' 기준 정의
    private const int MAX_STR_NORMAL = 6;       // 일반적인 힘 단련 한계
    private const int STR_ULTIMATE = 7;         // 인간 들기 해금 시 힘 수치
    private const int MAX_BAG_WEIGHT = 5100;    // 가방 최대 무게 (예: 5000에서 10번 업그레이드 시)
    private const int MAX_HP_LIMIT = 200;       // 최대 청결도 한계 (예: 100에서 10번 업그레이드 시)
    private const float MIN_PICK_SPEED = 0.25f; // 속도 한계 (이 이하로 내려가면 MAX)
    private const int MAX_GRAB_LIMIT = 5;       // 흡입구 개수 한계

    // [NEW] 모든 스탯이 최대치에 도달했는지 검사하는 함수
    public bool IsAllStatMaxed()
    {
        if (GameManager.instance == null) return false;

        // 모든 조건이 참(True)이어야 최종 True 반환
        bool isStrMax = GameManager.instance.P_Str >= MAX_STR_NORMAL;
        bool isBagMax = GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT;
        bool isHpMax = GameManager.instance.P_MaxHP >= MAX_HP_LIMIT;
        // 속도는 낮을수록 빠름 (0.25 이하면 MAX)
        bool isSpeedMax = GameManager.instance.grab_speed <= MIN_PICK_SPEED;
        bool isGrabMax = GameManager.instance.grab_limit >= MAX_GRAB_LIMIT;

        return isStrMax && isBagMax && isHpMax && isSpeedMax && isGrabMax;
    }

    public int GetUpgradeCost(UpgradeType type)
    {
        if (GameManager.instance == null) return 0;

        switch (type)
        {
            case UpgradeType.Strength:
                // 이미 6 이상이면 비용 0 (구매 불가 UI 처리용)
                if (GameManager.instance.P_Str >= MAX_STR_NORMAL) return 0;
                return GameManager.instance.P_Str * baseStrengthCost;

            case UpgradeType.BagWeight:
                if (GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT) return 0;
                return (GameManager.instance.P_Maxbag / 10) * baseBagCost;

            case UpgradeType.MaxHP:
                if (GameManager.instance.P_MaxHP >= MAX_HP_LIMIT) return 0;
                return (GameManager.instance.P_MaxHP / 10) * baseMaxHPCost;

            case UpgradeType.PickSpeed:
                if (GameManager.instance.grab_speed <= MIN_PICK_SPEED) return 0;
                float progress = 1.5f - GameManager.instance.grab_speed;
                int level = Mathf.RoundToInt(progress * 10);
                return baseSpeedCost + (level * 500);

            case UpgradeType.MultiGrab:
                if (GameManager.instance.grab_limit >= MAX_GRAB_LIMIT) return 0;
                int currentLimit = GameManager.instance.grab_limit;
                return baseMultiGrabCost + ((currentLimit - 1) * 1000);

            case UpgradeType.PickNPC:
                // 조건: 모든 스탯 만렙이고, 아직 힘이 7이 아닐 때만 가격 표시
                if (IsAllStatMaxed() && GameManager.instance.P_Str < STR_ULTIMATE)
                    return pickNpcCost;
                return 0; // 조건 불충족 시 0원

            default:
                return 0;
        }
    }

    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        if (GameManager.instance == null) return false;

        // 1. 최대치 제한 체크 (일반 업그레이드 막기)
        if (type == UpgradeType.Strength && GameManager.instance.P_Str >= MAX_STR_NORMAL) return false;
        if (type == UpgradeType.BagWeight && GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT) return false;
        if (type == UpgradeType.MaxHP && GameManager.instance.P_MaxHP >= MAX_HP_LIMIT) return false;
        if (type == UpgradeType.PickSpeed && GameManager.instance.grab_speed <= MIN_PICK_SPEED) return false;
        if (type == UpgradeType.MultiGrab && GameManager.instance.grab_limit >= MAX_GRAB_LIMIT) return false;

        // 2. 인간 들기 구매 조건 체크
        if (type == UpgradeType.PickNPC)
        {
            if (!IsAllStatMaxed())
            {
                Debug.Log("조건 부족: 모든 능력을 한계까지 강화하십시오.");
                return false;
            }
            if (GameManager.instance.P_Str >= STR_ULTIMATE)
            {
                Debug.Log("이미 인간을 들 수 있는 힘을 가졌습니다.");
                return false;
            }
        }

        // 3. 비용 확인 및 결제
        int cost = GetUpgradeCost(type);
        if (cost == 0) return false; // 구매 불가 상태

        if (GameManager.instance.P_Money < cost)
        {
            Debug.Log($"돈이 부족합니다! (필요: {cost})");
            return false;
        }

        GameManager.instance.P_Money -= cost;

        // 4. 능력치 적용
        switch (type)
        {
            case UpgradeType.Strength:
                GameManager.instance.P_Str += 1;
                Debug.Log($"힘 증가: {GameManager.instance.P_Str}");
                break;

            case UpgradeType.BagWeight:
                GameManager.instance.P_Maxbag += 10;
                Debug.Log($"가방 확장: {GameManager.instance.P_Maxbag}");
                break;

            case UpgradeType.MaxHP:
                GameManager.instance.P_MaxHP += 10;
                GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP; // 체력 회복 서비스
                Debug.Log($"청결도 확장: {GameManager.instance.P_MaxHP}");
                break;

            case UpgradeType.PickSpeed:
                GameManager.instance.grab_speed -= 0.1f;
                if (GameManager.instance.grab_speed < 0.2f) GameManager.instance.grab_speed = 0.2f;
                Debug.Log($"속도 강화: {GameManager.instance.grab_speed}");
                break;

            case UpgradeType.MultiGrab:
                GameManager.instance.grab_limit += 1;
                GameManager.instance.grab_range = 0.2f + ((GameManager.instance.grab_limit - 1) * 0.15f);
                Debug.Log($"흡입구 확장: {GameManager.instance.grab_limit}개");
                break;

            case UpgradeType.PickNPC:
                // [핵심] 한계 돌파! 힘을 7로 설정
                GameManager.instance.P_Str = STR_ULTIMATE;
                Debug.Log(" 초월 업그레이드 완료! 인간을 들 수 있습니다! (힘 7 달성) ");
                break;
        }

        GameManager.instance.SaveAllGamedata();
        return true;
    }
}
