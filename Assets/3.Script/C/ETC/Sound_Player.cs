using UnityEngine;
using UnityEngine.UI;

public class Sound_Player : MonoBehaviour
{
    [Header("재생할 소리 이름")]
    [SerializeField] private string soundName = "UI_Click"; // 기본값

    private Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        // 버튼 클릭 이벤트에 소리 재생 함수 연결
        if (btn != null)
        {
            btn.onClick.AddListener(PlaySound);
        }
    }

    private void PlaySound()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.PlaySFX(soundName);
        }
    }
}
