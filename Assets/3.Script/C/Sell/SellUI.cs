using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SellUI : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private SellManager sellManager;
    [SerializeField] private PlayerController playerController;

    [Header("UI 패널 연결")]
    [SerializeField] private GameObject tradePanel;   // 견적서
    [SerializeField] private GameObject resultPanel;  // 결과창

    [Header("메인 HUD 연결 (추가됨)")]
    [Tooltip("상점 이용 중에 숨길 메인 HUD (체력바, 미니맵, 우상단 돈 등)")]
    [SerializeField] private GameObject mainHudPanel;

    [Header("돈 텍스트 연결")]
    [Tooltip("결과 창 안에 있는 '총 보유 금액' 텍스트 (애니메이션용)")]
    [SerializeField] private Text txtResultTotalMoney;

    // (선택) 견적서에 있는 돈 텍스트
    [SerializeField] private Text txtTradeCurrentMoney;

    [Header("기타 UI 요소")]
    [SerializeField] private Text infoText;
    [SerializeField] private Text resultText;

    [Header("버튼 연결")]
    [SerializeField] private Button sellButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

    [Header("연출 설정")]
    [SerializeField] private float animationDuration = 1.0f;

    private Coroutine moneyCoroutine;

    public bool IsUIActive
    {
        get
        {
            bool isTradeOpen = tradePanel != null && tradePanel.activeSelf;
            bool isResultOpen = resultPanel != null && resultPanel.activeSelf;
            return isTradeOpen || isResultOpen;
        }
    }

    private void Awake()
    {
        // 시작 시 초기화
        CloseAllPanels();

        if (sellButton != null) sellButton.onClick.AddListener(OnClickSell);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnClickCancel);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnClickConfirm);
    }

    // 1. 상점 열기
    public void OpenTradePanel()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (tradePanel != null) tradePanel.SetActive(true);

        // [추가] 상점 이용 중에는 메인 HUD를 숨겨서 화면을 깔끔하게 합니다.
        if (mainHudPanel != null) mainHudPanel.SetActive(false);

        if (AudioManager.instance != null) AudioManager.instance.PlaySFX("SFX11");

        if (sellManager != null)
        {
            int smallEarn = sellManager.GetSmallTrashEarnings();
            int bigEarn = sellManager.GetBigTrashEarnings();
            int totalEarn = smallEarn + bigEarn;

            // 견적서 창의 돈 텍스트 갱신
            if (txtTradeCurrentMoney != null && GameManager.instance != null)
                txtTradeCurrentMoney.text = $"{GameManager.instance.P_Money:N0}";

            if (infoText != null)
            {
                infoText.text = $"Small Trash: {smallEarn:N0} G\n" +
                                $"Big Trash: {bigEarn:N0} G\n" +
                                $"----------------\n" +
                                $"Total: {totalEarn:N0} G";
            }
        }
        SetPlayerControl(false);
    }

    // 2. 판매 버튼 클릭
    private void OnClickSell()
    {
        if (sellManager != null)
        {
            if (AudioManager.instance != null) AudioManager.instance.PlaySFX("SFX10");

            // [A] 시작 금액 (현재 돈)
            int startMoney = 0;
            if (GameManager.instance != null) startMoney = GameManager.instance.P_Money;

            // [B] 실제 판매 처리 (돈 증가)
            int earnings = sellManager.SellAllTrash();

            // [C] 목표 금액 (늘어난 돈)
            int targetMoney = startMoney + earnings;

            // [D] 결과창 띄우기 (애니메이션 포함)
            ShowResult(earnings, startMoney, targetMoney);
        }
    }

    private void ShowResult(int earnedMoney, int startMoney, int targetMoney)
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);

        // [확인] 여기서도 HUD는 계속 꺼져있어야 합니다. (OpenTradePanel에서 이미 껐음)
        if (mainHudPanel != null) mainHudPanel.SetActive(false);

        // "+ 1500 G" 텍스트
        if (resultText != null)
            resultText.text = $"Success!\n<size=60><color=yellow>+ {earnedMoney:N0} G</color></size>";

        // 결과창 내부 총 보유액 애니메이션
        if (txtResultTotalMoney != null)
        {
            if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
            moneyCoroutine = StartCoroutine(AnimateResultMoney(startMoney, targetMoney));
        }
    }

    // 결과창 전용 돈 애니메이션
    private IEnumerator AnimateResultMoney(int start, int target)
    {
        float elapsed = 0f;

        if (txtResultTotalMoney != null) txtResultTotalMoney.text = $"{start:N0}";

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animationDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // EaseOut

            int currentDisplay = (int)Mathf.Lerp(start, target, t);

            if (txtResultTotalMoney != null)
                txtResultTotalMoney.text = $"{currentDisplay:N0}";

            yield return null;
        }

        if (txtResultTotalMoney != null) txtResultTotalMoney.text = $"{target:N0}";
    }

    // 3. 확인 버튼 & 닫기
    private void OnClickConfirm() => CloseAllPanels();
    private void OnClickCancel() => CloseAllPanels();

    public void CloseAllPanels()
    {
        if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);

        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        // [추가] 상점을 완전히 나갈 때 HUD를 다시 켜줍니다.
        if (mainHudPanel != null) mainHudPanel.SetActive(true);

        SetPlayerControl(true);
    }

    private void SetPlayerControl(bool isActive)
    {
        if (playerController != null)
        {
            playerController.enabled = isActive;

            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        if (isActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
