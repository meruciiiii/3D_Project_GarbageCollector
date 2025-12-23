using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowerSystem : MonoBehaviour
{
    [Header("샤워 설정")]
    [SerializeField] private int showerCost = 100;
    [SerializeField] private float showerDuration = 2.0f;
    [SerializeField] private GameObject guideText;

    [Header("샤워 연출 (FX & Audio)")] // [NEW] 연출용 변수 추가
    [SerializeField] private ParticleSystem waterFX; // 물방울 파티클
    [SerializeField] private ParticleSystem steamFX; // 수증기 파티클
    [SerializeField] private AudioSource showerAudio; // 물소리 오디오
    [SerializeField] private EyeOpenClose showerEyeEffect;

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

        // [NEW] 시작 시 이펙트가 켜져있다면 강제로 끔 (안전장치)
        if (waterFX != null) waterFX.Stop();
        if (steamFX != null) steamFX.Stop();
        if (showerAudio != null) showerAudio.Stop();
    }

    public void TryShower()
    {
        if (GameManager.instance != null && GameManager.instance.isPaused)
        {
            return;
        }
        if (GameManager.instance.P_Money < showerCost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        StartCoroutine(ProcessShower());
    }

    private IEnumerator ProcessShower()
    {
        isWashing = true;

        // 1. 플레이어 얼리기 (커서는 여전히 안 보여야 함!)
        SetPlayerFreeze(true);

        // [NEW] 2. 샤워 연출 시작 (Play)
        if (waterFX != null) waterFX.Play();
        if (steamFX != null) steamFX.Play();
        if (showerAudio != null) showerAudio.Play();
        if (showerEyeEffect != null) showerEyeEffect.CloseEyes();

        Debug.Log("샤워 시작...");
        if (guideText != null) guideText.SetActive(false);

        // 데이터 처리
        GameManager.instance.P_Money -= showerCost;
        GameManager.instance.ChangeHP(GameManager.instance.P_MaxHP);
        GameManager.instance.SaveAllGamedata();

        // 3. 씻는 시간 대기 (연출 감상 시간)
        yield return new WaitForSeconds(showerDuration);

        // 4. 화면 효과 초기화
        if (vignetteController != null) vignetteController.OnWash();
        if (playerIsDirty != null) playerIsDirty.StopDirtyEffect();
        if (showerEyeEffect != null) showerEyeEffect.OpenEyes();

        Debug.Log("샤워 완료!");

        // [NEW] 5. 샤워 연출 종료 (Stop)
        if (waterFX != null) waterFX.Stop();
        if (steamFX != null) steamFX.Stop(); // 수증기는 Stop하면 자연스럽게 잔상이 사라집니다.
        if (showerAudio != null) showerAudio.Stop();

        // 6. 플레이어 녹이기
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
