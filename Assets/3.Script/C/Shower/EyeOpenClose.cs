using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class EyeOpenClose : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 2.0f;

    private void Awake() // Start 대신 Awake 권장
    {
        // 안전장치: 컴포넌트가 없으면 찾기
        if (fadeImage == null) fadeImage = GetComponent<Image>();
    }

    // Start()에서 SetActive(false)를 하면 첫 실행 시 꼬일 수 있으므로
    // 초기화 로직은 '값이 튀지 않게' 투명도만 잡아줍니다.
    private void Start()
    {
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f; // 투명하게 시작
            fadeImage.color = c;
            // 여기서 gameObject.SetActive(false)를 하지 마세요! 
            // 인스펙터에서 미리 꺼두거나, CloseEyes()가 알아서 처리하게 둡니다.
        }
    }

    public void CloseEyes()
    {
        this.gameObject.SetActive(true);

        // 안전장치: fadeImage가 따로 있다면 그것도 켭니다.
        if (fadeImage != null) fadeImage.gameObject.SetActive(true);

        StopAllCoroutines();

        // 깜빡임 방지: 시작할 때 투명(0)으로 확실히 초기화
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        StartCoroutine(FadeProcess(0f, 1f));
    }

    public void OpenEyes()
    {
        this.gameObject.SetActive(true);

        if (fadeImage != null) fadeImage.gameObject.SetActive(true);

        StopAllCoroutines();
        StartCoroutine(FadeProcess(1f, 0f));
    }

    private IEnumerator FadeProcess(float startAlpha, float endAlpha)
    {
        float timer = 0f;
        Color c = fadeImage.color;

        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeSpeed;
            c.a = Mathf.Lerp(startAlpha, endAlpha, timer);
            fadeImage.color = c;
            yield return null;
        }

        // 최종값 고정
        c.a = endAlpha;
        fadeImage.color = c;

        // 완전히 투명해졌다면(눈을 떴다면) 오브젝트를 꺼서 성능 최적화
        if (endAlpha <= 0.01f)
        {
            fadeImage.gameObject.SetActive(false);
        }
    }
}
