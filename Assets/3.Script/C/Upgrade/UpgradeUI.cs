using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UpgradeUI : MonoBehaviour
{
    [Header("시스템 연결")]
    [SerializeField] private UpgradeManager upgradeManager;
    [SerializeField] private PlayerController playerController;

    [Header("UI 연결")]
    [SerializeField] private Text txtMyMoney; // 돈 표시 텍스트 (프리팹 안의 Text 연결)
    [SerializeField] private GameObject panel; // 전체 패널
    [SerializeField] private Button closeButton;

    [Header("슬롯 목록")]
    [SerializeField] private List<UpgradeSlot> slots;

    [Header("연출 설정")]
    [SerializeField] private float animationDuration = 0.5f; // 돈 바뀌는 속도 (0.5초)

    private Coroutine moneyCoroutine;
    private int currentDisplayMoney = 0; // 현재 화면에 표시되고 있는 가짜 돈 값

    private void Awake()
    {
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(CloseShop);
        }

        // 모든 슬롯 초기화
        foreach (var slot in slots)
        {
            slot.Setup(upgradeManager, this);
        }
    }

    private void OnEnable()
    {
        // 상점 열릴 때: 애니메이션 없이 바로 현재 돈 딱 보여줌
        if (GameManager.instance != null)
        {
            currentDisplayMoney = GameManager.instance.P_Money;
            UpdateMoneyText(currentDisplayMoney);
        }

        RefreshAll(false); // false = 애니메이션 없이 갱신
        SetPlayerControl(false);
    }

    private void OnDisable()
    {
        SetPlayerControl(true);
    }

    // ---------------------------------------------------------
    // 화면 갱신 함수 (animate: true면 숫자가 굴러감)
    // ---------------------------------------------------------
    public void RefreshAll(bool animate = true)
    {
        if (GameManager.instance == null) return;

        // 1. 돈 갱신 (핵심)
        int targetMoney = GameManager.instance.P_Money;

        if (animate && gameObject.activeInHierarchy)
        {
            // 돈을 썼으면 애니메이션 실행
            if (moneyCoroutine != null) StopCoroutine(moneyCoroutine);
            moneyCoroutine = StartCoroutine(AnimateMoneyRoutine(currentDisplayMoney, targetMoney));
        }
        else
        {
            // 상점 처음 켤 때는 그냥 바로 표시
            currentDisplayMoney = targetMoney;
            UpdateMoneyText(currentDisplayMoney);
        }

        // 2. 슬롯들 갱신 (구매 버튼 활성/비활성 등)
        foreach (var slot in slots)
        {
            slot.Refresh();
        }
    }

    // ---------------------------------------------------------
    // 돈 숫자 굴러가는 코루틴
    // ---------------------------------------------------------
    private IEnumerator AnimateMoneyRoutine(int start, int target)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.unscaledDeltaTime; // UI니까 unscaledTime 사용
            float t = elapsed / animationDuration;

            // Lerp로 중간값 계산
            currentDisplayMoney = (int)Mathf.Lerp(start, target, t);
            UpdateMoneyText(currentDisplayMoney);

            yield return null;
        }

        // 끝난 후 정확한 값 고정
        currentDisplayMoney = target;
        UpdateMoneyText(currentDisplayMoney);
    }

    private void UpdateMoneyText(int value)
    {
        if (txtMyMoney != null)
        {
            txtMyMoney.text = $"{value:N0}"; // 15,000 형식
        }
    }

    private void SetPlayerControl(bool isActive)
    {
        if (playerController != null)
        {
            playerController.enabled = isActive;
            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }
            Cursor.visible = !isActive;
            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
}


