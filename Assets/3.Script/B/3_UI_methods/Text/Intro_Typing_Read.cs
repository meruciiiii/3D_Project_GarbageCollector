using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro_Typing_Read : MonoBehaviour
{
    [SerializeField] private Text text_component;
    [SerializeField] private string Key1;
    [SerializeField] private float typingSpeed = 0.05f; // 글자 출력 속도
    [SerializeField] private float lineWaitTime = 0.6f; // 줄바꿈 시 대기 시간
    [SerializeField] private float waitforstart = 0f; // 줄바꿈 시 대기 시간


    [SerializeField] private string nextSceneName = "B_Scene"; // 인트로 종료 후 갈 씬
    [SerializeField] private bool isclear = false;
    [SerializeField] private Image fadeImage; // 페이드용 UI 이미지
    [SerializeField] private float fadeDuration = 1.5f; // 페이드 속도


    private string Key2 = "value";
    private Coroutine loadCoroutine;

    void OnEnable()
    {
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
        loadCoroutine = StartCoroutine(WaitForDataAndTyping());
    }

    private IEnumerator WaitForDataAndTyping()
    {
        yield return new WaitForSeconds(waitforstart);
        // 1. 데이터 로딩 대기
        while (GameManager.instance == null || !GameManager.instance.LoadComplete) yield return null;
        while (CSV_Database.instance == null || !CSV_Database.instance.IsLoaded) yield return null;

        if (string.IsNullOrEmpty(Key1) || CSV_Database.instance.DataMap == null) yield break;

        // 2. 데이터 가져오기
        if (CSV_Database.instance.DataMap.TryGetValue(Key1, out Dictionary<string, object> Data))
        {
            if (Data.ContainsKey(Key2))
            {
                string fullText = Data[Key2].ToString();//데이터 값 빼오기

                yield return StartCoroutine(TypeText(fullText));

                // 3. 타이핑이 끝나면 잠시 대기 후 다음 씬으로 이동
                GameManager.instance.P_intro = false;
                yield return new WaitForSeconds(2.0f);
                if (isclear) yield return new WaitForSeconds(5.0f);
                yield return StartCoroutine(FadeOutRoutine());
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
    private IEnumerator FadeOutRoutine()
    {
        if (fadeImage == null) yield break;

        fadeImage.gameObject.SetActive(true); // 이미지 활성화
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            Color c = fadeImage.color;
            c.a = Mathf.Lerp(0f, 1f, timer / fadeDuration); // 0(투명) -> 1(검정)
            fadeImage.color = c;
            yield return null;
        }

        // 완전히 검게 확정
        Color finalColor = fadeImage.color;
        finalColor.a = 1f;
        fadeImage.color = finalColor;
    }

    private int characterCounter = 0; // 글자 수를 세는 변수

    private IEnumerator TypeText(string message)
    {
        text_component.text = ""; // 텍스트 초기화
        characterCounter = 0;    // 카운터 초기화

        foreach (char letter in message.ToCharArray())
        {
            if (letter == '|') //줄바꿈
            {
                text_component.text += "\n";
                yield return new WaitForSeconds(lineWaitTime);
                continue;
            }

            text_component.text += letter;
            if (letter != ' ')
            {
                characterCounter++;

                // 카운터가 지정한 간격이 되면 소리 재생
                if (characterCounter >= Random.Range(2, 4)) // 2글자 혹은 3글자마다 랜덤하게 재생
                {
                    AudioManager.instance.PlaySFX("SFX2");
                    characterCounter = 0;
                }
            }
            float randomSpeed = Random.Range(typingSpeed * 0.8f, typingSpeed * 1.2f);
            yield return new WaitForSeconds(randomSpeed);
        }
    }

    void OnDisable()
    {
        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
    }
}
