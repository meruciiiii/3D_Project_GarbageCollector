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
    [SerializeField] private Sound[] BGM_clip;
    [Space(10f)]
    [SerializeField] private Sound[] SFX_clip;

    [Space(50f)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource BGM_Player;
    [SerializeField] private AudioSource[] SFX_Player;

    [Space(50f)]
    [SerializeField] private float fadeDuration = 1.0f; // 페이드 속도 조절 변수
    private Coroutine bgmFadeCoroutine; // 현재 실행 중인 페이드 관리

    private void AutoSetting()
    {
        BGM_Player = transform.GetChild(0).GetComponent<AudioSource>();
        SFX_Player = transform.GetChild(1).GetComponentsInChildren<AudioSource>();
    }

    public void PlayBGM(string name)
    {
        foreach(Sound s in BGM_clip)
        {
            if (s.name.Equals(name))
            {
                if (BGM_Player.clip == s.clip) return;
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
        if (BGM_Player.isPlaying)
        {
            while (BGM_Player.volume > 0)
            {
                BGM_Player.volume -= Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        // 2. 클립 교체
        BGM_Player.clip = newClip;
        BGM_Player.outputAudioMixerGroup = BGM;
        BGM_Player.loop = true;
        BGM_Player.Play();

        // 3. Fade In
        while (BGM_Player.volume < 1.0f)
        {
            BGM_Player.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
        BGM_Player.volume = 1.0f;
    }

    public void StopBGM()
    {
        BGM_Player.Stop();
        BGM_Player.clip = null;
    }

    /*
    public void PlaySFX(string name)
    {
        foreach(Sound s in SFX_clip)
        {
            if (s.name.Equals(name))
            {
                for (int i = 0; i < SFX_Player.Length; i++)
                {
                    if (!SFX_Player[i].isPlaying)
                    {
                        SFX_Player[i].clip = s.clip;
                        SFX_Player[i].outputAudioMixerGroup = SFX;
                        SFX_Player[i].Play();
                        return;
                    }
                }
                Debug.Log("모든 Audio Source SFXPlayer가 Play중");
                return;
            }
        }
    }
    */

    public void PlaySFX(string name, Vector3 position) //AudioManager.instance.PlaySFX("사운드이름",transform.position)
    {
        foreach(Sound s in SFX_clip)
        {
            if (s.name.Equals(name))
            {
                for (int i = 0; i < SFX_Player.Length; i++)
                {
                    if (!SFX_Player[i].isPlaying)
                    {
                        SFX_Player[i].transform.position = position;
                        SFX_Player[i].clip = s.clip;
                        SFX_Player[i].outputAudioMixerGroup = SFX;
                        SFX_Player[i].Play();
                        return;
                    }
                }
                Debug.Log("모든 슬롯이 사용 중입니다.");
                return;
            }
        }
        Debug.Log($"해당 name:[{name}] key를 가진 SFX가 없습니다.");
    }
}
