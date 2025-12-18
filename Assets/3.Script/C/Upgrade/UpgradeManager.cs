using UnityEngine;

public enum UpgradeType
{
    Strength,   // 힘
    BagWeight,  // 가방 무게
    MaxHP      // 이동 속도
}
public class UpgradeManager : MonoBehaviour
{
    [Header("업그레이드 가격 설정")]
    [SerializeField] private int baseStrengthCost = 1000;
    [SerializeField] private int baseBagCost = 500;
    [SerializeField] private int baseMaxHPCost = 1500;

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

            default:
                return 0;
        }
    }
    public bool TryPurchaseUpgrade(UpgradeType type)
    {
        if (GameManager.instance == null) return false;

        int cost = GetUpgradeCost(type);
        int currentMoney = GameManager.instance.P_Money;

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
        }

        // 저장 (GameManager의 저장 메서드 호출)
        GameManager.instance.SaveAllGamedata();

        return true; // 성공 리턴
    }
}
