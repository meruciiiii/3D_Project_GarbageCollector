using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Sceneload : MonoBehaviour
{
    [Header("설정")]
    [SerializeField] private float buttonDelay = 0.68f; // 사운드 대기
    [SerializeField] private float fadeDuration = 0.5f; // 페이드 아웃 시간
    [SerializeField] private Image fadeImage;          // 화면을 가릴 검은색 이미지

    public void SceneLoader(string scenename)
    {
        StartCoroutine(LoadAsync(scenename));
    }

    IEnumerator LoadAsync(string scenename)
    {
        // 1. 버튼 사운드 대기
        yield return new WaitForSeconds(buttonDelay);

        // 2. 페이드 아웃 (점점 어두워짐)
        if (fadeImage != null)
        {
            fadeImage.gameObject.SetActive(true);
            float timer = 0f;
            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                Color c = fadeImage.color;
                c.a = timer / fadeDuration;
                fadeImage.color = c;
                yield return null;
            }
        }

        string finalSceneName = scenename;

        if (GameManager.instance != null && GameManager.instance.P_intro)
        {
            finalSceneName = "B_Intro"; // 인트로 씬의 이름은 추후 수정가능
            GameManager.instance.P_intro = false;
        }

        // 3. 비동기 씬 로드
        AsyncOperation op = SceneManager.LoadSceneAsync(finalSceneName);

        while (!op.isDone)
        {
            yield return null;
        }
    }
}
