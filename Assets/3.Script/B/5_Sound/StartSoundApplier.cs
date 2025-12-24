using UnityEngine;
using UnityEngine.Audio;
using System.Collections;

public class StartSoundApplier : MonoBehaviour
{
    [SerializeField] private AudioMixer audiomixer;
    IEnumerator Start()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Apply("BGM");
        Apply("SFX");
        Apply("Master");
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
