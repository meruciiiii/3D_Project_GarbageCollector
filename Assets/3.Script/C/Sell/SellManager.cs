using UnityEngine;

public class SellManager : MonoBehaviour
{
    [Header("정산 설정")]
    [Tooltip("1kg당 판매 가격 (무게 단위가 크므로 1원으로 설정)")]
    [SerializeField] private int pricePerWeight = 1;

    [Tooltip("대형 쓰레기 보너스 (힘들게 들고 왔으니 1.2배)")]
    [SerializeField] private float bigTrashMultiplier = 1.2f;

    [SerializeField] private PlayerController playerController;

    public int GetSmallTrashEarnings()
    {
        if (GameManager.instance == null) return 0;
        return GameManager.instance.P_Weight * pricePerWeight;
    }

    public int GetBigTrashEarnings()
    {
        if (GameManager.instance == null) return 0;

        if (GameManager.instance.isGrabBigGarbage)
        {
            // 대형 쓰레기는 보너스 적용
            float earnings = GameManager.instance.BigGarbageWeight * pricePerWeight * bigTrashMultiplier;
            return Mathf.RoundToInt(earnings);
        }
        return 0;
    }

    // 3. 총 합계 계산 (기존 함수 유지하되 내부에서 위 함수들 활용)
    public int CalculatePotentialEarnings()
    {
        return GetSmallTrashEarnings() + GetBigTrashEarnings();
    }

    // 실제 판매 로직 (기존 코드 유지 + 삭제 로직 포함)
    public int SellAllTrash()
    {
        if (GameManager.instance == null) return 0;

        int smallEarn = GetSmallTrashEarnings();
        int bigEarn = GetBigTrashEarnings();
        int totalEarnings = smallEarn + bigEarn;

        // --- 데이터 초기화 및 삭제 로직 ---

        // [A] 소형 정산
        if (smallEarn > 0)
        {
            GameManager.instance.P_Weight = 0;
        }

        // [B] 대형 정산
        if (bigEarn > 0)
        {
            // 오브젝트 삭제
            BigTrashAction catcher = FindAnyObjectByType<BigTrashAction>();
            if (catcher != null) catcher.SellHeldTrash();

            GameManager.instance.isGrabBigGarbage = false;
            GameManager.instance.BigGarbageWeight = 0;
        }

        if (totalEarnings > 0 && playerController != null)
        {
            playerController.SendMessage("Walk", SendMessageOptions.DontRequireReceiver);
        }

            // --- 돈 지급 및 저장 ---
            if (totalEarnings > 0)
        {
            GameManager.instance.P_Money += totalEarnings;
            GameManager.instance.SaveAllGamedata();
            Debug.Log($"정산 완료: 소형({smallEarn}) + 대형({bigEarn}) = {totalEarnings}원");
        }

        return totalEarnings;
    }
}
