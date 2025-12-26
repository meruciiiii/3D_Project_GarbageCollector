using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SellUI : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private SellManager sellManager;

    [Header("플레이어 연결")]
    [SerializeField] private PlayerController playerController;

    [Header("UI 패널 연결")]
    [SerializeField] private GameObject tradePanel;   // 거래 전 견적서 창
    [SerializeField] private GameObject resultPanel;  // 거래 후 결과 창

    [Header("UI 요소 연결")]
    [SerializeField] private Text CurrentMoney;       // [UI] 현재 돈을 보여줄 텍스트 (여기에 값 넣음)
    [SerializeField] private Text infoText;           // 상세 내역
    [SerializeField] private Text resultText;         // 결과
    [SerializeField] private Button sellButton;
    [SerializeField] private Button cancelButton;
    [SerializeField] private Button confirmButton;

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
        CloseAllPanels();
        if (sellButton != null) sellButton.onClick.AddListener(OnClickSell);
        if (cancelButton != null) cancelButton.onClick.AddListener(OnClickCancel);
        if (confirmButton != null) confirmButton.onClick.AddListener(OnClickConfirm);
    }

    // 1. 견적서 열기
    public void OpenTradePanel()
    {
        if (resultPanel != null) resultPanel.SetActive(false);
        if (tradePanel != null) tradePanel.SetActive(true);

        if (sellManager != null)
        {
            int smallEarn = sellManager.GetSmallTrashEarnings();
            int bigEarn = sellManager.GetBigTrashEarnings();
            int totalEarn = smallEarn + bigEarn;

            // [핵심] GameManager에서 돈을 가져와서 UI에 표시
            UpdateMoneyUI();

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
            int earnings = sellManager.SellAllTrash();
            ShowResult(earnings);
        }
    }

    // 3. 결과창 보여주기
    private void ShowResult(int earnedMoney)
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(true);

        // 결과창에서도 갱신된 돈을 보여주고 싶다면 호출 (선택사항)
        // UpdateMoneyUI(); 

        if (resultText != null)
        {
            resultText.text = $"Success!\n<size=60><color=yellow>+ {earnedMoney:N0} G</color></size>";
        }
    }

    // [중요] GameManager에서 돈을 가져오는 전용 함수
    private void UpdateMoneyUI()
    {
        // 1. 텍스트 UI가 연결되어 있고
        // 2. GameManager가 존재한다면
        if (CurrentMoney != null && GameManager.instance != null)
        {
            // GameManager의 P_Money 값을 가져와서 텍스트로 변환
            // N0 포맷은 3자리마다 콤마를 찍어줍니다. (예: 15,000)
            CurrentMoney.text = $"{GameManager.instance.P_Money:N0}";
        }
        else
        {
            // 예외 처리 (디버깅용)
            // Debug.LogWarning("CurrentMoney UI 연결 안됨 또는 GameManager 없음");
        }
    }

    // ... (나머지 닫기 및 컨트롤 함수들은 기존과 동일) ...
    private void OnClickCancel() => CloseAllPanels();
    private void OnClickConfirm() => CloseAllPanels();

    public void CloseAllPanels()
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);
        SetPlayerControl(true);
    }

    private void SetPlayerControl(bool isActive)
    {
        if (playerController != null)
        {
            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            playerController.enabled = isActive;
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
