using UnityEngine;

public enum UpgradeType
{
    Strength,   // 힘
    BagWeight,  // 가방 무게
    Speed       // 이동 속도
}
public class UpgradeManager : MonoBehaviour
{
    [Header("업그레이드 가격 설정")]
    [SerializeField] private int baseStrengthCost = 1000;
    [SerializeField] private int baseBagCost = 500;
    [SerializeField] private int baseSpeedCost = 2000;

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
                // 예: (현재 가방크기 / 10) * 기본가격
                return (GameManager.instance.P_Maxbag / 10) * baseBagCost;

            case UpgradeType.Speed:
                return baseSpeedCost; // 고정 가격

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

            case UpgradeType.Speed:
                GameManager.instance.P_Spd += 1;
                break;
        }

        // 저장 (GameManager의 저장 메서드 호출)
        GameManager.instance.SaveAllGamedata();

        return true; // 성공 리턴
    }
}
