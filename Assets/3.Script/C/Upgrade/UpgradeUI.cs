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
    [SerializeField] private Text txtMyMoney; // 내 돈 표시
    [SerializeField] private GameObject panel; // 전체 패널

    [Header("슬롯 목록 (인스펙터에서 넣어주세요)")]
    [SerializeField] private List<UpgradeSlot> slots;

    private void Awake()
    {
        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();

        // 모든 슬롯 초기화
        foreach (var slot in slots)
        {
            slot.Setup(upgradeManager, this);
        }
    }

    private void OnEnable()
    {
        RefreshAll();
        SetPlayerControl(false);
    }

    private void OnDisable()
    {
        SetPlayerControl(true);
    }

    // 모든 슬롯과 내 돈 갱신
    public void RefreshAll()
    {
        // 1. 돈 갱신
        if (GameManager.instance != null && txtMyMoney != null)
        {
            txtMyMoney.text = $"{GameManager.instance.P_Money}";
        }

        // 2. 슬롯들 갱신
        foreach (var slot in slots)
        {
            slot.Refresh();
        }
    }

    private void SetPlayerControl(bool isActive)
    {
        if (playerController != null)
        {
            playerController.enabled = isActive;

            // 물리 정지
            if (!isActive)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null) rb.linearVelocity = Vector3.zero;
            }

            // 마우스 커서
            Cursor.visible = !isActive;
            Cursor.lockState = isActive ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    public void CloseShop()
    {
        gameObject.SetActive(false);
    }
}


