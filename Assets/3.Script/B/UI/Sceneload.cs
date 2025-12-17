using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Sceneload : MonoBehaviour
{
    public void SceneLoader(string scenename) //버튼용 메소드 입니다.
    {
        StartCoroutine(LoadSceneandButtonDelay(scenename));
    }
    public IEnumerator LoadSceneandButtonDelay(string scenename)//바로 로드되면 버튼 사운드가 끊겨서 그 사운드에 맞춰서
    {
        yield return new WaitForSeconds(0.68f);// 이 시간을 맞춰 수정해주세요.
        Debug.Log("SceneLoad!");
        SceneManager.LoadScene(scenename);
    }
}
