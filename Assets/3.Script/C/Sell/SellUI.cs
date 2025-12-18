using UnityEngine;
using UnityEngine.UI;

public class SellUI : MonoBehaviour
{
    [Header("매니저 연결")]
    [SerializeField] private SellManager sellManager;

    [SerializeField] private PlayerController playerController;

    [Header("UI 패널 연결")]
    [SerializeField] private GameObject tradePanel;   // 거래 대기 창
    [SerializeField] private GameObject resultPanel;  // 결과 확인 창

    [Header("UI 요소 연결")]
    [SerializeField] private Text infoText;           // "보유 무게: 10kg..."
    [SerializeField] private Text resultText;         // "1500원 획득!"
    [SerializeField] private Button sellButton;       // 판매 버튼
    [SerializeField] private Button cancelButton;     // 취소 버튼
    [SerializeField] private Button confirmButton;    // 확인 버튼 (결과창)

    private void Awake()
    {
        // 시작 시 UI 모두 끄기
        CloseAllPanels();

        // 버튼 리스너 연결
        if (sellButton != null) sellButton.onClick.AddListener(OnClick_Sell);
        if (cancelButton != null) cancelButton.onClick.AddListener(CloseAllPanels);
        if (confirmButton != null) confirmButton.onClick.AddListener(CloseAllPanels);

        // 플레이어 컨트롤러 자동 찾기 (혹시 연결 안했을 경우)
        if (playerController == null)
        {
            // 주의: 팀원의 스크립트 이름이 FirstPersonMovement가 맞는지 확인 필요
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
                playerController = player.GetComponent<PlayerController>(); // 혹은 다른 이동 스크립트
        }
    }

    // 상점 UI 열기 (외부 호출용)
    public void OpenSellMenu()
    {
        if (tradePanel == null) return;

        SetPlayerControl(false); // 플레이어 멈춤
        tradePanel.SetActive(true);
        resultPanel.SetActive(false);

        UpdateTradeInfo();
    }

    // 정보 갱신
    private void UpdateTradeInfo()
    {
        if (GameManager.instance == null) return;

        int weight = GameManager.instance.P_Weight;
        int money = sellManager != null ? sellManager.CalculatePotentialEarnings() : 0;

        if (infoText != null)
            infoText.text = $"현재 무게: {weight} kg\n예상 금액: <color=yellow>{money} G</color>";
    }

    // 판매 버튼 클릭 시
    public void OnClick_Sell()
    {
        if (sellManager == null) return;

        int earned = sellManager.SellAllTrash();

        // 0원이라도 팔았다고 쳐야 하는지, 아니면 팔게 없다고 해야하는지 결정
        // 여기서는 금액이 0보다 클 때만 결과창을 띄웁니다.
        if (earned > 0)
        {
            ShowResult(earned);
        }
        else
        {
            // 팔 것이 없을 때 그냥 닫거나, "팔 물건이 없습니다" 메시지 띄우기
            CloseAllPanels();
        }
    }

    private void ShowResult(int money)
    {
        tradePanel.SetActive(false);
        resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = $"정산이 완료되었습니다!\n\n<color=yellow>+{money} G</color>";
    }

    // 모든 패널 닫기 (취소/확인)
    public void CloseAllPanels()
    {
        if (tradePanel != null) tradePanel.SetActive(false);
        if (resultPanel != null) resultPanel.SetActive(false);

        SetPlayerControl(true); // 플레이어 다시 움직임
    }

    // 플레이어 움직임/커서 제어
    private void SetPlayerControl(bool isActive)
    {
        // 1. 플레이어 이동 스크립트 켜고 끄기
        if (playerController != null)
        {
            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero; // Unity 6 (구버전은 velocity)
                    rb.angularVelocity = Vector3.zero;
                }
            }

            playerController.enabled = isActive;
        }

        // 2. 마우스 커서 설정
        if (isActive)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked; // 게임 중엔 잠금
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None; // UI 떴을 땐 풀기
        }
    }
}
