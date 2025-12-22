using UnityEngine;

public enum UpgradeType
{
    Strength,   // 힘
    BagWeight,  // 가방 무게
    MaxHP,      // 이동 속도
    PickSpeed, // 줍기 속도
    MultiGrab // 줍기 개수
}
public class UpgradeManager : MonoBehaviour
{
    [Header("업그레이드 가격 설정")]
    [SerializeField] private int baseStrengthCost = 1000;
    [SerializeField] private int baseBagCost = 500;
    [SerializeField] private int baseMaxHPCost = 1500;
    [SerializeField] private int baseSpeedCost = 2000;
    [SerializeField] private int baseMultiGrabCost = 3000;

    public int GetUpgradeCost(UpgradeType type)
    {
        // GameManager가 로딩되지 않았으면 0 반환 (안전장치)
        if (GameManager.instance == null) return 0;

        switch (type)
        {
            case UpgradeType.Strength:
                // 예: 현재 힘 * 기본가격 (점점 비싸짐)
                return GameManager.instance.P_Str * baseStrengthCost;

            case UpgradeType.BagWeight:
            //    // 예: (현재 가방크기 / 10) * 기본가격
                return (GameManager.instance.P_Maxbag / 10) * baseBagCost;

            case UpgradeType.MaxHP:
                return (GameManager.instance.P_MaxHP / 10) * baseMaxHPCost;

            case UpgradeType.PickSpeed:
                float progress = 1.5f - GameManager.instance.grab_speed;
                int level = Mathf.RoundToInt(progress * 10);
                return baseSpeedCost + (level * 500);

            case UpgradeType.MultiGrab:
                // 현재 줍기 개수 제한에 따라 가격 증가
                // 1개(기본) -> 2개(레벨1) -> 3개...
                int currentLimit = GameManager.instance.grab_limit;
                // 예: (현재개수 - 1) * 1000원 추가
                return baseMultiGrabCost + ((currentLimit - 1) * 1000);

            default:
                return 0;
        }
    }
    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        if (GameManager.instance == null) return false;

        int cost = GetUpgradeCost(type);
        int currentMoney = GameManager.instance.P_Money;

        if (type == UpgradeType.PickSpeed && GameManager.instance.grab_speed <= 0.25f)
        {
            Debug.Log("더 이상 속도를 업그레이드할 수 없습니다. (MAX)");
            return false;
        }

        if (type == UpgradeType.MultiGrab && GameManager.instance.grab_limit >= 5)
        {
            Debug.Log("더 이상 흡입구를 확장할 수 없습니다.");
            return false;
        }

        // 돈 확인
        if (currentMoney < cost)
        {
            Debug.Log($"돈이 부족합니다! (보유: {currentMoney}, 필요: {cost})");
            return false;
        }

        // --- 거래 성사 ---

        // 돈 차감 (GameManager 변수 직접 수정)
        GameManager.instance.P_Money -= cost;

        // 능력치 증가 (GameManager 변수 직접 수정)
        switch (type)
        {
            case UpgradeType.Strength:
                GameManager.instance.P_Str += 1;
                Debug.Log($"힘 업그레이드! 현재 힘: {GameManager.instance.P_Str}");
                break;

            case UpgradeType.BagWeight:
                GameManager.instance.P_Maxbag += 10; // 10kg 증가
                Debug.Log($"가방 확장! 현재 용량: {GameManager.instance.P_Maxbag}");
                break;

            case UpgradeType.MaxHP:
                // [변경] 최대 청결도 10 증가
                GameManager.instance.P_MaxHP += 10;

                // (선택사항) 업그레이드 기념으로 현재 청결도도 꽉 채워주기
                GameManager.instance.P_CurrentHP = GameManager.instance.P_MaxHP;

                Debug.Log($"최대 청결도 확장 완료! 현재 Max: {GameManager.instance.P_MaxHP}");
                break;

            case UpgradeType.PickSpeed:
                // [NEW] 쿨타임 0.1초 감소
                GameManager.instance.grab_speed -= 0.1f;

                // 부동 소수점 오차 방지 및 하한선 고정
                if (GameManager.instance.grab_speed < 0.2f) GameManager.instance.grab_speed = 0.2f;

                Debug.Log($"줍기 속도 강화! 현재 딜레이: {GameManager.instance.grab_speed}초");
                break;

            case UpgradeType.MultiGrab:
                // [핵심] 줍는 개수 제한을 1 늘립니다.
                GameManager.instance.grab_limit += 1;
                float newRange = 0.2f + ((GameManager.instance.grab_limit - 1) * 0.15f);

                // GameManager 변수에 값 주입
                GameManager.instance.grab_range = newRange;
                Debug.Log($"흡입구 확장! 한 번에 {GameManager.instance.grab_limit}개 수거 가능");
                break;
        }

        // 저장 (GameManager의 저장 메서드 호출)
        GameManager.instance.SaveAllGamedata();

        return true; // 성공 리턴
    }
}
