using UnityEngine;

public class SellManager : MonoBehaviour
{
    [Header("정산 설정")]
    [Tooltip("1 무게당 얼마를 줄 것인가?")]
    [SerializeField] private int pricePerWeight = 15; // 예: 1kg당 15원

    // 예상 금액 계산
    public int CalculatePotentialEarnings()
    {
        if (GameManager.instance == null) return 0;
        return GameManager.instance.P_Weight * pricePerWeight;
    }

    // 실제 판매 및 데이터 저장
    public int SellAllTrash()
    {
        if (GameManager.instance == null) return 0;

        int currentWeight = GameManager.instance.P_Weight;

        // 판매할 게 없으면 0 리턴
        if (currentWeight <= 0) return 0;

        // 가격 계산
        int earnings = currentWeight * pricePerWeight;

        // 데이터 적용 (돈 추가, 무게 초기화)
        GameManager.instance.P_Money += earnings;
        GameManager.instance.P_Weight = 0;

        // 저장
        GameManager.instance.SaveAllGamedata();
        Debug.Log($"[SellManager] 정산 완료: {earnings}원 획득 / 무게 초기화됨");

        return earnings;
    }
}
