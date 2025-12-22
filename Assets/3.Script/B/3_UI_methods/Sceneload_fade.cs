using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BSceneManager : MonoBehaviour
{
    [SerializeField] private Image fadeImage; // 화면을 가릴 UI 이미지 (검은색)
    [SerializeField] private float waitTimeForTrash = 2.5f; // 쓰레기가 떨어질 때까지 기다릴 시간

    private PlayerController playerController;
    private bool isFirstLoad = false;

    private void Awake()
    {
        // 시작하자마자 화면을 완전히 가림
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f;
            fadeImage.color = c;
        }
        playerController = FindAnyObjectByType<PlayerController>();
    }
        
    //void Start()와는 다르게, 시작하자마자 코루틴(Coroutine)처럼 동작
    private void OnEnable()
    {
        StartCoroutine(fade());
    }

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
            playerController.enabled = !isFrozen;
        }

        // 커서 코드는 건드리지 않거나, 확실하게 잠급니다.
        if (isFrozen)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private IEnumerator fade()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            if (isFirstLoad)
            {
                Color c = fadeImage.color;
                c.a = 0f;
                fadeImage.color = c;
            }
        }

        yield return null;
        SetPlayerFreeze(true);

        if (fadeImage != null&& isFirstLoad)
        {
            Debug.Log("화면 어두워지는 중 (Fade Out)");
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime; // 1초 동안 어두워짐
                Color c = fadeImage.color;
                c.a = timer; // 0(투명)에서 1(검정)로
                fadeImage.color = c;
                yield return null;
            }
        }
        if(!isFirstLoad) isFirstLoad = true;

        // 1. 이 상태에서 이미 B씬의 물리(쓰레기 낙하)는 작동 중
        Debug.Log("쓰레기 낙하 시작 (화면 가려짐)");

        // 2. 쓰레기가 충분히 떨어질 때까지 기다림
        yield return new WaitForSeconds(waitTimeForTrash);//IEnumerator 처럼  yield return new WaitForSeconds 사용가능

        // 3. 서서히 화면을 밝게 함 (페이드 인)
        if (fadeImage != null)
        {
            float timer = 0f;
            while (timer < 1f)
            {
                timer += Time.deltaTime;
                Color c = fadeImage.color;
                c.a = 1f - timer;
                fadeImage.color = c;
                yield return null;
            }
            fadeImage.gameObject.SetActive(false);
        }
        Debug.Log("연출 완료, 화면 공개");
        
        SetPlayerFreeze(false);
        this.gameObject.SetActive(false);
    }

}