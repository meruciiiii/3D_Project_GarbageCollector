using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class SellUI : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private SellManager sellManager; // 계산을 담당하는 매니저

    [Header("플레이어 연결")]
    [SerializeField] private PlayerController playerController;

    [Header("UI 패널 연결")]
    [SerializeField] private GameObject tradePanel;   // 거래 전 견적서 창
    [SerializeField] private GameObject resultPanel;  // 거래 후 결과 창

    [Header("UI 요소 연결")]
    [SerializeField] private Text infoText;           // 상세 내역 텍스트
    [SerializeField] private Text resultText;         // 결과 텍스트 ("+1500 G")
    [SerializeField] private Button sellButton;       // 판매 버튼
    [SerializeField] private Button cancelButton;     // 취소 버튼
    [SerializeField] private Button confirmButton;    // 확인 버튼 (결과창 닫기)

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
        // 1. 시작할 때 UI가 켜져있으면 안 되므로 모두 닫기
        CloseAllPanels();

        // 2. 버튼 기능 연결 (Inspector에서 연결 안 해도 코드에서 자동 연결)
        if (sellButton != null) sellButton.onClick.AddListener(OnClick_Sell);
        if (cancelButton != null) cancelButton.onClick.AddListener(CloseAllPanels);
        if (confirmButton != null) confirmButton.onClick.AddListener(CloseAllPanels);

        // 3. 플레이어 컨트롤러가 연결 안 되어있으면 자동으로 찾기 (안전장치)
        if (playerController == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerController = player.GetComponent<PlayerController>();
        }
    }

    // ---------------------------------------------------------
    // 1. 외부(SellStation)에서 호출하여 상점 열기
    // ---------------------------------------------------------
    public void OpenSellMenu()
    {
        if (tradePanel == null) return;

        // 플레이어 조작 멈춤
        SetPlayerControl(false);

        // 패널 켜기
        tradePanel.SetActive(true);
        resultPanel.SetActive(false);

        // 가격 정보 최신화해서 보여주기
        UpdateTradeInfo();
    }

    // ---------------------------------------------------------
    // 2. 가격 정보를 계산해서 텍스트로 보여주는 함수 (영수증 기능)
    // ---------------------------------------------------------
    private void UpdateTradeInfo()
    {
        if (GameManager.instance == null || sellManager == null || infoText == null) return;

        // [1] SellManager에서 각각의 금액을 계산해옴
        int smallEarn = sellManager.GetSmallTrashEarnings();
        int bigEarn = sellManager.GetBigTrashEarnings();
        int totalEarn = smallEarn + bigEarn;

        // [2] GameManager에서 무게 정보 가져옴
        int smallWeight = GameManager.instance.P_Weight;
        // 대형 쓰레기가 없으면 무게도 0으로 취급
        int bigWeight = GameManager.instance.isGrabBigGarbage ? GameManager.instance.BigGarbageWeight : 0;

        // [3] 영수증 텍스트 만들기
        string receipt = "";

        // 소형 쓰레기 줄
        receipt += $"Small Trash ({smallWeight}kg) :  <color=white>{smallEarn} G</color>\n";

        // 대형 쓰레기 줄 (들고 있을 때만 초록색 강조)
        if (GameManager.instance.isGrabBigGarbage)
        {
            receipt += $"Big Trash ({bigWeight}kg) :  <color=#00FF00>+ {bigEarn} G</color>\n";
        }
        else
        {
            receipt += $" No Big Trash :  <color=#808080>0 G</color>\n";
        }

        //receipt += "--------------------------------\n";
        receipt += $"Result : <color=yellow>{totalEarn} G</color>";

        // [4] 화면에 적용
        infoText.text = receipt;
    }

    // ---------------------------------------------------------
    // 3. 판매 버튼 눌렀을 때 실행
    // ---------------------------------------------------------
    public void OnClick_Sell()
    {
        if (sellManager == null) return;

        // 실제 정산 실행 (돈 받고, 아이템 삭제)
        int earned = sellManager.SellAllTrash();

        // 번 돈이 있다면 결과창 표시
        if (earned > 0)
        {
            ShowResult(earned);
        }
        else
        {
            // 팔 게 없으면 그냥 닫기 (또는 안내 메시지 표시 가능)
            CloseAllPanels();
        }
    }

    // 결과창 띄우기
    public void ShowResult(int earnings)
    {
        // [수정] 1. 뒤에 있는 정산 견적서 창(TradePanel)을 꺼줘야 함!
        if (tradePanel != null) tradePanel.SetActive(false);

        // 2. 결과창 켜기
        if (resultPanel != null) resultPanel.SetActive(true);

        string resultMessage = "Settlement complete";

        // CSV 데이터 로드 (아까 추가한 로직)
        if (CSV_Database.instance != null && CSV_Database.instance.DataMap != null)
        {
            if (CSV_Database.instance.DataMap.TryGetValue("ui_result", out Dictionary<string, object> data))
            {
                resultMessage = data["value"].ToString();
            }
        }

        // 텍스트 갱신
        if (resultText != null)
        {
            resultText.text = $"{resultMessage}\n<color=yellow>+ {earnings} G</color>";
        }
    }

    // ---------------------------------------------------------
    // 4. 상점 닫기 (취소/확인)
    // ---------------------------------------------------------
    public void CloseAllPanels()
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        // 플레이어 조작 다시 활성화
        SetPlayerControl(true);
    }

    // ---------------------------------------------------------
    // 5. 플레이어 조작 및 커서 제어 (물리 미끄러짐 방지 포함)
    // ---------------------------------------------------------
    private void SetPlayerControl(bool isActive)
    {
        // [A] 플레이어 이동 스크립트 제어
        if (playerController != null)
        {
            // UI가 켜질 때(=게임 멈춤, !isActive) 물리 속도 제거
            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    // Unity 6버전 이상에서는 linearVelocity 사용
                    // 구버전(Unity 2022 이하)이라면 rb.velocity 로 변경하세요.
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }

            // 스크립트 켜고 끄기
            playerController.enabled = isActive;
        }

        // [B] 마우스 커서 제어
        if (isActive)
        {
            // 게임 플레이 중: 커서 숨김 & 고정
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            // UI 조작 중: 커서 보임 & 자유 이동
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
