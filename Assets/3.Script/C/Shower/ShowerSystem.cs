using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowerSystem : MonoBehaviour
{
    [Header("샤워 설정")]
    [SerializeField] private int showerCost = 100;
    [SerializeField] private float showerDuration = 2.0f;
    [SerializeField] private GameObject guideText;

    // FirstPersonMovement로 타입 명시 추천
    [SerializeField] private PlayerController playerController;
    [SerializeField] private VignetteController vignetteController;
    [SerializeField] private PlayerIsDirty playerIsDirty;

    private PlayerInput targetInput;
    private bool isWashing = false;

    // ... (Start, OnTriggerEnter 등 기존 코드 유지) ...

    private void Start()
    {
        if (guideText != null) guideText.SetActive(false);
    }

    public void TryShower()
    {
        if (GameManager.instance == null) return;
        if (GameManager.instance.P_Money < showerCost) return; // 돈 부족 등 예외처리

        StartCoroutine(ProcessShower());
    }

    private IEnumerator ProcessShower()
    {
        isWashing = true;

        // 1. 플레이어 얼리기 (커서는 여전히 안 보여야 함!)
        SetPlayerFreeze(true);

        Debug.Log("샤워 시작...");
        if (guideText != null) guideText.SetActive(false);

        GameManager.instance.P_Money -= showerCost;
        GameManager.instance.ChangeHP(GameManager.instance.P_MaxHP);
        GameManager.instance.SaveAllGamedata();

        // 2. 씻는 시간 대기
        yield return new WaitForSeconds(showerDuration);

        vignetteController.OnWash();
        playerIsDirty.StopDirtyEffect();

        Debug.Log("샤워 완료!");

        // 4. 플레이어 녹이기
        SetPlayerFreeze(false);
        isWashing = false;

        // 아직 범위 안이면 텍스트 다시 켜기
        if (targetInput != null && guideText != null) guideText.SetActive(true);
    }

    //샤워 전용 상태 관리 함수
    private void SetPlayerFreeze(bool isFrozen)
    {
        if (playerController != null)
        {
            if (isFrozen)
            {
                Rigidbody rb = playerController.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero; // Unity 6 (구버전은 velocity)
                    rb.angularVelocity = Vector3.zero;
                }
            }

            // 얼릴 때(true)는 enabled = false
            // 녹일 때(false)는 enabled = true
            playerController.enabled = !isFrozen;
        }

        // 커서 코드는 건드리지 않거나, 확실하게 잠급니다.
        if (isFrozen)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    // ... (OnTriggerEnter/Exit 이벤트 구독 로직 유지) ...
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (guideText != null) guideText.SetActive(true);
            targetInput = other.GetComponent<PlayerInput>();

            // 플레이어 컨트롤러 캐싱
            if (playerController == null)
                playerController = other.GetComponent<PlayerController>();

            if (targetInput != null) targetInput.onInteract += TryShower;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (guideText != null) guideText.SetActive(false);
            if (targetInput != null)
            {
                targetInput.onInteract -= TryShower;
                targetInput = null;
            }
        }
    }
}
