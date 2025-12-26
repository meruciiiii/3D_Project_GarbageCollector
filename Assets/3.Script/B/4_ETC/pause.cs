using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject Pause_UI;
    [SerializeField] private GameObject fullmap;
    [SerializeField] private GameObject minimap;
    [SerializeField] private GameObject sellUI;

    [Header("플레이어 연결")] private PlayerController controller;

    private PlayerInput input;

    private void Awake()
    {
        input = FindAnyObjectByType<PlayerInput>();
        controller = FindAnyObjectByType<PlayerController>();
        //if (input == null) Debug.Log("input null임");
        //else Debug.Log("input 받아옴");
    }

    private void Start()
    {
        input.onEsc += TogglePause;
    }

    public void TogglePause() // UI 버튼에 할당도 가능
    {
        if (Pause_UI == null) return;

        UpgradeUI shopUI = FindAnyObjectByType<UpgradeUI>();
        if (shopUI != null && shopUI.gameObject.activeInHierarchy)
        {
            Debug.Log("업그레이드 패널이 열려있어 일시정지를 할 수 없습니다.");
            return;
        }

        if (sellUI != null && sellUI.gameObject.activeInHierarchy)
        {
            Debug.Log("정산 패널이 열려있어 일시정지를 할 수 없습니다.");
            return;
        }
        AudioManager.instance.PlaySFX("SFX3");
        bool isCurrentlyActive = Pause_UI.activeSelf;
        bool targetState = !isCurrentlyActive;

        Pause_UI.SetActive(targetState);
        fullmap.SetActive(targetState);
        minimap.SetActive(!targetState);

        // [핵심] SellUI와 동일한 로직 적용
        SetPlayerControl(!targetState);
    }

    private void SetPlayerControl(bool isActive)
    {
        if (GameManager.instance != null) GameManager.instance.isPaused = !isActive; //퍼즈 상황

        if (controller != null)
        {
            // UI가 켜질 때(!isActive) 카메라와 이동을 멈춤
            if (!isActive)
            {
                Rigidbody rb = controller.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            controller.enabled = isActive;
        }

        // 마우스 커서 제어 (isActive가 true면 게임 중, false면 UI 조작 중)
        if (isActive)
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null); //resume 버튼 하이라이트 재사용을 위한 선택 초기화
            }
            Cursor.visible = false;
            Time.timeScale = 1f; // 게임 재개
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Time.timeScale = 0f; // 게임 일시정지 (물리 멈춤)
            Cursor.lockState = CursorLockMode.None;
        }
    }
}
