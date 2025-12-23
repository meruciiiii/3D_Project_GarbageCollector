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
    [Header("업그레이드 가격 설정 (Base Cost)")]
    [SerializeField] private int baseStrengthCost = 50000; // 힘: 매우 비쌈 (새 지역 해금급)
    [SerializeField] private int baseBagCost = 5000;       // 가방: 1/3 ~ 1회 수익
    [SerializeField] private int baseMaxHPCost = 10000;    // 체력: 적당함
    [SerializeField] private int baseSpeedCost = 15000;    // 속도: 체감 큼
    [SerializeField] private int baseMultiGrabCost = 100000;// 멀티: 후반용
    [SerializeField] private int pickNpcCost = 1000000;    // 히든: 100만 골드 (엔딩)

    // [데이터 기반 졸업 기준 재설정]
    private const int MAX_STR_NORMAL = 6;
    private const int STR_ULTIMATE = 7;

    // [중요] small_9(12000) 2개는 담아야 함 -> 30,000까지 확장
    private const int MAX_BAG_WEIGHT = 30000;
    private const int MAX_HP_LIMIT = 200;
    private const float MIN_PICK_SPEED = 0.25f;
    private const int MAX_GRAB_LIMIT = 5;

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
        int level = 0;

        switch (type)
        {
            case UpgradeType.Strength: // 힘 (1->2가 가장 중요)
                if (GameManager.instance.P_Str >= MAX_STR_NORMAL) return 0;
                // 1->2: 5만, 2->3: 20만, 3->4: 45만...
                return baseStrengthCost * (GameManager.instance.P_Str * GameManager.instance.P_Str);

            case UpgradeType.BagWeight: // 가방 (5000 ~ 30000)
                if (GameManager.instance.P_Maxbag >= MAX_BAG_WEIGHT) return 0;
                // (현재 - 5000) / 2500 -> 단계 계산
                level = (GameManager.instance.P_Maxbag - 5000) / 2500;
                // 5000 + (단계 * 3000) -> 점진적 증가
                return baseBagCost + (level * 3000);

            case UpgradeType.MaxHP:
                if (GameManager.instance.P_MaxHP >= MAX_HP_LIMIT) return 0;
                level = (GameManager.instance.P_MaxHP - 100) / 10;
                return baseMaxHPCost + (level * 5000);

            case UpgradeType.PickSpeed: // 속도 (1.5 -> 0.25)
                if (GameManager.instance.grab_speed <= MIN_PICK_SPEED) return 0;
                // 안전장치 추가: 혹시 1.5보다 크면 0레벨로 취급
                float startSpeed = 1.5f;
                if (GameManager.instance.grab_speed > 1.5f) startSpeed = GameManager.instance.grab_speed;

                // (1.5 - 현재) / 0.1 -> 약 12단계
                level = Mathf.RoundToInt((startSpeed - GameManager.instance.grab_speed) / 0.1f);
                if (level < 0) level = 0;
                return baseSpeedCost + (level * 5000);

            case UpgradeType.MultiGrab:
                if (GameManager.instance.grab_limit >= MAX_GRAB_LIMIT) return 0;
                // 10만, 20만, 30만...
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
        // ... (조건 체크 및 결제 로직 기존 동일) ...
        int cost = GetUpgradeCost(type);
        if (cost == 0 || GameManager.instance.P_Money < cost) return false;

        GameManager.instance.P_Money -= cost;

        switch (type)
        {
            case UpgradeType.Strength:
                GameManager.instance.P_Str += 1;
                break;

            case UpgradeType.BagWeight:
                // [변경] +50은 너무 적음. +2500씩 시원하게 확장 (약 10번 업글)
                GameManager.instance.P_Maxbag += 2500;
                Debug.Log($"가방 확장: {GameManager.instance.P_Maxbag}");
                break;

            case UpgradeType.MaxHP:
                GameManager.instance.P_MaxHP += 10;
                GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP;
                break;

            case UpgradeType.PickSpeed:
                // [변경] 0.1씩 감소
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
