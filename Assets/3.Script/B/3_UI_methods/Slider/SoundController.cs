using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;


public class SoundController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider[] sliders;

    private const float snapValue = 5f; // 5단위로 스냅

    void Start()
    {
        // AudioManager에 저장된 값을 가져와서 슬라이더 초기화
        InitializeSlider(0, "BGM");
        InitializeSlider(1, "SFX");
        InitializeSlider(2, "Master");
    }

    private void InitializeSlider(int index, string parameter)
    {
        if (index < sliders.Length && sliders[index] != null)
        {
            sliders[index].value = AudioManager.instance.GetSavedVolume(parameter);
        }
    }

    public void ControllVolume(string audioGroup)
    {
        int index = -1;//선택이 없으면 -1로 들어가서 default
        switch (audioGroup)
        {
            case "BGM": index = 0; break;
            case "SFX": index = 1; break;
            case "Master": index = 2; break;
            default:
                Debug.LogWarning($"{audioGroup}은(는) 잘못된 이름입니다.");
                return;
        }

        float snappedValue = Mathf.Round(sliders[index].value / snapValue) * snapValue; //5단위 반올림
        sliders[index].value = snappedValue;

        AudioManager.instance.SetVolume(audioGroup, snappedValue);
    }
}
