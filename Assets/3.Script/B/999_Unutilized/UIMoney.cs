using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIMoney : MonoBehaviour
{
    [SerializeField] private Text moneyText;
    [SerializeField] private float animationDuration = 0.5f;

    // [수정] 가짜돈 타입을 int로 변경
    private int currentDisplayMoney = 0;
    private Coroutine moneyCoroutine;

    private void Start()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UIValueChange += UpdateMoneyUI;
        }

        StartCoroutine(WaitForGameManager());
    }

    private void OnEnable()
    {
        UpdateMoneyUI();
    }

    private IEnumerator WaitForGameManager()
    {
        while (GameManager.instance == null || !GameManager.instance.LoadComplete)
            yield return null;

        // [수정] 나누기 없이 정수 원본 사용
        currentDisplayMoney = GameManager.instance.P_Money;
        moneyText.text = $"{currentDisplayMoney:N0}$";

        UIManager.instance.change_Value();
    }

    public void UpdateMoneyUI()
    {
        if (GameManager.instance == null || !GameManager.instance.LoadComplete) return;
        if (!gameObject.activeInHierarchy) return;

        // [수정] 타겟 금액도 정수형으로 가져옴
        int targetMoney = GameManager.instance.P_Money;

        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
        moneyCoroutine = StartCoroutine(AnimateMoney(targetMoney));
    }

    private IEnumerator AnimateMoney(int target)
    {
        float elapsed = 0f;
        int startValue = currentDisplayMoney;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            // Lerp 계산 후 int로 캐스팅하여 정수만 취함
            currentDisplayMoney = (int)Mathf.Lerp(startValue, target, elapsed / animationDuration);

            // [수정] N0 포맷으로 소수점 제거 및 콤마 표시
            moneyText.text = $"{currentDisplayMoney:N0}$";
            yield return null;
        }

        currentDisplayMoney = target;
        moneyText.text = $"{target:N0}$";
    }

    private void OnDestroy()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.UIValueChange -= UpdateMoneyUI;
        }
    }
}