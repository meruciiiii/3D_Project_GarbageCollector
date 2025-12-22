using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class pause : MonoBehaviour
{
    [SerializeField] private GameObject Pause_UI;
    [SerializeField] private GameObject fullmap;

    [Header("플레이어 연결")] private PlayerController controller;

    private PlayerInput input;
    private bool ispause = false;

    private void Awake()
    {
        input = FindAnyObjectByType<PlayerInput>();
        controller = FindAnyObjectByType<PlayerController>();
        if (input == null) Debug.Log("input null임");
        else Debug.Log("input 받아옴");
    }

    private void Start()
    {
        input.onInteract += TogglePause; //차후 ESC 인풋 이벤트에 등록
    }

    public void TogglePause() // UI 버튼에 할당도 가능
    {
        if (Pause_UI == null || fullmap == null) return;

        bool isCurrentlyActive = Pause_UI.activeSelf;
        bool targetState = !isCurrentlyActive;

        Pause_UI.SetActive(targetState);
        fullmap.SetActive(targetState);

        // [핵심] SellUI와 동일한 로직 적용
        SetPlayerControl(!targetState);
    }

    private void SetPlayerControl(bool isActive)
    {
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
            Cursor.lockState = CursorLockMode.Locked;
            Time.timeScale = 1f; // 게임 재개
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 0f; // 게임 일시정지 (물리 멈춤)
        }
    }
}
