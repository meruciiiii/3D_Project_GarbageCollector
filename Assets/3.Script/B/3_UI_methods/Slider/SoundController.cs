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
        LoadAndApplyVolume("BGM", 0);
        LoadAndApplyVolume("SFX", 1);
    }

    private void LoadAndApplyVolume(string parameter, int SliderIndex)
    {
        string key = "volume_" + parameter;
        float savedValue = PlayerPrefs.GetFloat(key, 75f); //기본값 75

        sliders[SliderIndex].value = savedValue;

        ApplyToMixer(parameter, savedValue);
    }

    private void ApplyToMixer(string parameter, float sliderValue)
    {
        // 0~100의 값을 0.0001~1 범위로 변환
        float normalizeValue = Mathf.Clamp(sliderValue / 100f, 0.0001f, 1f);

        // 로그 변환을 통해 인간의 귀에 자연스러운 dB 값으로 변경 (-80dB ~ 0dB)
        float dB = Mathf.Log10(normalizeValue) * 20;

        audioMixer.SetFloat(parameter, dB);
    }

    public void ControllVolume(string audioGroup)
    {
        int index = 0;
        if (audioGroup == "BGM") index = 0;
        else if (audioGroup == "SFX") index = 1;
        else { Debug.Log("audioGroup 이름이 없습니다."); return; }

        float snappedValue = Mathf.Round(sliders[index].value / snapValue) * snapValue; //5단위 반올림
        sliders[index].value = snappedValue;

        ApplyToMixer(audioGroup, snappedValue);

        PlayerPrefs.SetFloat("volume_" + audioGroup, snappedValue);
        PlayerPrefs.Save();
    }
}
