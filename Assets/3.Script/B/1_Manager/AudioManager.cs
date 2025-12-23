using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[Serializable]//데이터 직렬화
public class Sound
{
    public string name;
    public AudioClip clip;
}
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance = null;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        AutoSetting();
    }

    [Header("AudioGroup")]
    [SerializeField] private AudioMixerGroup BGM;
    [SerializeField] private AudioMixerGroup SFX;

    [Space(10f)]
    [Header("AudioClip")]
    [SerializeField] private Sound[] BGMclip;
    [SerializeField] private Sound[] SFXclip;

    [Space(50f)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource BGMPlayer;
    [SerializeField] private AudioSource[] SFXPlayer;

    [Space(50f)]
    [SerializeField] private float fadeDuration = 1.0f; // 페이드 속도 조절 변수
    private Coroutine bgmFadeCoroutine; // 현재 실행 중인 페이드 관리

    private void AutoSetting()
    {
        BGMPlayer = transform.GetChild(0).GetComponent<AudioSource>();
        SFXPlayer = transform.GetChild(1).GetComponents<AudioSource>();
    }

    public void PlayBGM(string name)
    {
        foreach(Sound s in BGMclip)
        {
            if (s.name.Equals(name))
            {
                if (BGMPlayer.clip == s.clip) return;
                if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine);
                bgmFadeCoroutine = StartCoroutine(FadeBGM(s.clip));
                //BGMPlayer.clip = s.clip;
                //BGMPlayer.outputAudioMixerGroup = BGM; //오디오 믹서 그룹화
                //BGMPlayer.loop = true;//루프시킴
                //BGMPlayer.Play();
                break;
            }
        }
    }

    private IEnumerator FadeBGM(AudioClip newClip)
    {
        // 1. Fade Out
        if (BGMPlayer.isPlaying)
        {
            while (BGMPlayer.volume > 0)
            {
                BGMPlayer.volume -= Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        // 2. 클립 교체
        BGMPlayer.clip = newClip;
        BGMPlayer.outputAudioMixerGroup = BGM;
        BGMPlayer.loop = true;
        BGMPlayer.Play();

        // 3. Fade In
        while (BGMPlayer.volume < 1.0f)
        {
            BGMPlayer.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
        BGMPlayer.volume = 1.0f;
    }

    public void StopBGM()
    {
        BGMPlayer.Stop();
        BGMPlayer.clip = null;
    }

    public void PlaySFX(string name)
    {
        foreach(Sound s in SFXclip)
        {
            if (s.name.Equals(name))
            {
                for (int i = 0; i < SFXPlayer.Length; i++)
                {
                    if (!SFXPlayer[i].isPlaying)
                    {
                        SFXPlayer[i].clip = s.clip;
                        SFXPlayer[i].outputAudioMixerGroup = SFX;
                        SFXPlayer[i].Play();
                        return;
                    }
                }
                Debug.Log("모든 Audio Source SFXPlayer가 Play중");
                return;
            }
        }
        Debug.Log($"해당 name:[{name}] key를 가진 SFX가 없습니다.");
    }
}
