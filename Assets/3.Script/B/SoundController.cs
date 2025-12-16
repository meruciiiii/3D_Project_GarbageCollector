using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SoundController : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Slider[] sliders;

    void Start()
    {
        // BGM 볼륨 로드 및 적용
        if (PlayerPrefs.HasKey("volume_BGM"))
        {
            float bgmVolume = PlayerPrefs.GetFloat("volume_BGM");
            audioMixer.SetFloat("BGM", bgmVolume);
            sliders[0].value = bgmVolume; // 슬라이더 UI에도 적용
        }
        // SFX 볼륨 로드 및 적용
        if (PlayerPrefs.HasKey("volume_SFX"))
        {
            float sfxVolume = PlayerPrefs.GetFloat("volume_SFX");
            audioMixer.SetFloat("SFX", sfxVolume);
            sliders[1].value = sfxVolume; // 슬라이더 UI에도 적용
        }
        // PlayerPrefs에 값이 없을 경우, Unity가 설정한 슬라이더의 초기값(Start 시점의 슬라이더 value)이 사용
    }

    public void ControllVolume(string audioGroup)
    {
        float volumeValue = 0f;
        string parameter = "";

        switch (audioGroup)
        {
            case "BGM":
                volumeValue = sliders[0].value;
                parameter = "BGM";
                //audioMixer.SetFloat("BGM", sliders[0].value);
                break;

            case "SFX":
                volumeValue = sliders[1].value;
                parameter = "SFX";
                //audioMixer.SetFloat("SFX", sliders[1].value);
                break;
        }
        audioMixer.SetFloat(parameter, volumeValue);

        PlayerPrefs.SetFloat("volume_" + parameter, volumeValue);//키 volume_BGM 밸류 volumeValue
        PlayerPrefs.Save();
    }
}
