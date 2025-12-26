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
    [SerializeField] private string intro_name = "Main_intro";

    public void SceneLoader(string scenename)
    {
        if (Time.timeScale.Equals(0)) Time.timeScale = 1f;
        AudioManager.instance.PlaySFX("SFX1");
        StartCoroutine(LoadAsync(scenename));
    }

    private IEnumerator LoadAsync(string scenename)
    {
        // 1. 버튼 사운드 대기
        yield return new WaitForSeconds(buttonDelay);

        string finalSceneName = scenename;

        if (GameManager.instance != null && GameManager.instance.P_intro)
        {
            finalSceneName = intro_name;
        }

        AsyncOperation op = SceneManager.LoadSceneAsync(finalSceneName);
        op.allowSceneActivation = false; // 로딩이 완료되어도 자동으로 넘어가지 않게 설정

        // 2. 페이드 아웃 연출 시작
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
            Color finalColor = fadeImage.color;
            finalColor.a = 1f;
            fadeImage.color = finalColor;
        }

        // 3. 로딩 상태 확인 및 씬 전환 허용
        /*
         유니티의 AsyncOperation은 로딩 과정을 크게 두 단계

         데이터 로딩 (0% ~ 90%): 하드디스크에서 씬 데이터를 읽어오고 메모리에 올리는 실제 로딩 과정

         씬 활성화 (90% ~ 100%): 데이터를 다 읽은 후, 이전 씬을 메모리에서 지우고 새 씬의 오브젝트들을 화면에 배치하는 최종 마무리 과정
         */
        while (op.progress < 0.9f)
        {
            yield return null;
        }

        // 이제 모든 준비가 끝났으므로 씬 전환을 허락
        op.allowSceneActivation = true;

        // 최종적으로 전환될 때까지 대기
        while (!op.isDone)
        {
            yield return null;
        }
    }
}
