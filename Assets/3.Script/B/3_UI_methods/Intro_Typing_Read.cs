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
    [SerializeField] private string nextSceneName = "B_Scene"; // 인트로 종료 후 갈 씬

    private string Key2 = "value";
    private Coroutine loadCoroutine;

    void OnEnable()
    {
        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
        loadCoroutine = StartCoroutine(WaitForDataAndTyping());
    }

    private IEnumerator WaitForDataAndTyping()
    {
        // 1. 데이터 로딩 대기
        while (GameManager.instance == null || !GameManager.instance.LoadComplete) yield return null;
        while (CSV_Database.instance == null || !CSV_Database.instance.IsLoaded) yield return null;
        Debug.Log("intro");

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
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }

    private IEnumerator TypeText(string message)
    {
        text_component.text = ""; // 텍스트 초기화

        foreach (char letter in message.ToCharArray())
        {
            if (letter == '|') //줄바꿈
            {
                text_component.text += "\n";
                yield return new WaitForSeconds(lineWaitTime);
                continue;
            }

            text_component.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void OnDisable()
    {
        if (loadCoroutine != null) StopCoroutine(loadCoroutine);
    }
}
