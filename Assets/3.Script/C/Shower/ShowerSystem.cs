using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShowerSystem : MonoBehaviour
{
    [Header("샤워 설정")]
    [Tooltip("한 번 씻을 때 지불할 금액")]
    [SerializeField] private int showerCost = 100; // 예: 100원
    [Tooltip("씻는 데 걸리는 시간 (초)")]
    [SerializeField] private float showerDuration = 2.0f;

    [Header("연결 요소")]
    [SerializeField] private GameObject guideText;     // "F 눌러 씻기" 안내 UI

    // 플레이어 움직임 제어를 위해 필요하다면 연결 (없어도 됨)
    [SerializeField] private PlayerController playerController;

    private bool isPlayerNearby = false;
    private bool isWashing = false; // 현재 씻는 중인지 중복 방지 체크

    private void Start()
    {
        // 시작 시 안내 문구 꺼둡니다.
        if (guideText != null) guideText.SetActive(false);
    }

    private void Update()
    {
        // 1. 플레이어 근처 + 2. 씻는중 아님 + 3. F키 입력
        if (isPlayerNearby && !isWashing && Input.GetKeyDown(KeyCode.F))
        {
            TryShower();
        }
    }

    private void TryShower()
    {
        if (GameManager.instance == null) return;

        // [체크] 돈이 있는지?
        if (GameManager.instance.P_Money < showerCost)
        {
            Debug.Log($"돈이 부족합니다. (필요: {showerCost}, 보유: {GameManager.instance.P_Money})");
            return;
        }

        //씻기 시작
        StartCoroutine(ProcessShower());
    }

    private IEnumerator ProcessShower()
    {
        isWashing = true;

        // 1. 연출 시작
        Debug.Log("샤워 시작...");
        if (guideText != null) guideText.SetActive(false); // 씻는 동안 텍스트 끄기

        // (선택) 플레이어 못 움직이게 하기
        if (playerController != null) playerController.enabled = false;

        // 2. 시간 대기 (씻는 척)
        yield return new WaitForSeconds(showerDuration);

        // 3. 실제 데이터 적용 (돈 차감 + HP 회복)
        GameManager.instance.P_Money -= showerCost;

        // GameManager의 ChangeHP는 더하는 방식이므로 양수를 넣으면 회복됨
        // 1000처럼 큰 수를 넣어도 내부에서 Mathf.Clamp로 MaxHP까지만 차므로 안전함
        GameManager.instance.ChangeHP(GameManager.instance.P_MaxHP);

        // 4. 저장
        GameManager.instance.SaveAllGamedata();

        // 5. 연출 종료
        if (playerController != null) playerController.enabled = true;

        Debug.Log("샤워 완료! 청결도(HP)가 회복되었습니다.");

        isWashing = false;

        // 아직 부스 안에 있다면 안내 문구 다시 켜기
        if (isPlayerNearby && guideText != null) guideText.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = true;
            // 플레이어 컨트롤러 찾기 (매번 찾지 않으려면 Start에서 캐싱해도 됨)
            if (playerController == null)
                playerController = other.GetComponent<PlayerController>();

            if (guideText != null && !isWashing) guideText.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNearby = false;
            if (guideText != null) guideText.SetActive(false);

            // 씻다가 나가버리면? -> 코루틴은 계속 돌지만 isWashing 플래그 덕분에 문제는 없음
            // 원한다면 여기서 StopCoroutine을 할 수도 있습니다.
        }
    }
}
