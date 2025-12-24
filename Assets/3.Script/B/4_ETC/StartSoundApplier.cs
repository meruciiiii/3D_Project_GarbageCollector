using UnityEngine;
using UnityEngine.Audio;

public class StartSoundApplier : MonoBehaviour
{
    [SerializeField] private AudioMixer audiomixer;
    void Start()
    {
        // 게임 시작하자마자 저장된 볼륨을 불러와서 믹서에 적용
        Apply("BGM");
        Apply("SFX");
    }

    private void Apply(string parameter)
    {
        // 기존 SoundController와 동일한 로직
        float savedValue = PlayerPrefs.GetFloat("volume_" + parameter, 75f);

        float normalizeValue = Mathf.Clamp(savedValue / 100f, 0.0001f, 1f);
        float dB = Mathf.Log10(normalizeValue) * 20;

        audiomixer.SetFloat(parameter, dB);
    }

}
