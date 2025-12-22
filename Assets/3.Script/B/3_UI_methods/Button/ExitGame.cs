using UnityEngine;

public class ExitGame : MonoBehaviour
{
    public void exitGame()//게임 종료용 메서드입니다.
    {
        if (Time.timeScale.Equals(0)) Time.timeScale = 1f;
        Debug.Log("exitGame!!");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
