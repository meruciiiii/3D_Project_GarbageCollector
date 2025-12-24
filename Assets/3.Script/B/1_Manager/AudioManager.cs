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
    [SerializeField] private Sound[] SFX_3D_clip;

    [Space(50f)]
    [Header("Audio Source")]
    [SerializeField] private AudioSource BGM_Player;
    [SerializeField] private AudioSource[] SFX_Player;
    [SerializeField] private AudioSource[] SFX_3D_Player;

    [Space(50f)]
    [SerializeField] private float fadeDuration = 1.0f; // 페이드 속도 조절 변수
    private Coroutine bgmFadeCoroutine; // 현재 실행 중인 페이드 관리

    private Transform sfx3DContainer; //3D 플레이어 위치. 게임 오브젝트 파괴시 사라지는거 방지

    private void AutoSetting()
    {
        BGM_Player = transform.GetChild(0).GetComponent<AudioSource>();
        SFX_Player = transform.GetChild(1).GetComponents<AudioSource>();
        SFX_3D_Player = transform.GetChild(2).GetComponentsInChildren<AudioSource>();
        sfx3DContainer = transform.GetChild(2);
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

                /*
                 //기존 로직
                BGM_Player.clip = s.clip;
                BGM_Player.outputAudioMixerGroup = BGM; //오디오 믹서 그룹화
                BGM_Player.loop = true;//루프시킴
                BGM_Player.Play();
                */

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
        if (bgmFadeCoroutine != null) StopCoroutine(bgmFadeCoroutine); // 돌아가던 페이드도 멈춰줘야 함
        BGM_Player.Stop();
        BGM_Player.clip = null;
        BGM_Player.volume = 1.0f;
    }
    
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
    
    public void Play3DSFX(string name, Transform gameobject) //AudioManager.instance.Play3DSFX("3D_SFX", this.transform);
    {
        foreach(Sound s in SFX_3D_clip)
        {
            if (s.name.Equals(name))
            {
                for (int i = 0; i < SFX_3D_Player.Length; i++)
                {
                    if (!SFX_3D_Player[i].isPlaying)
                    {
                        SFX_3D_Player[i].transform.SetParent(gameobject); //자식으로 들어갑니다

                        SFX_3D_Player[i].transform.localPosition = Vector3.zero;
                        SFX_3D_Player[i].clip = s.clip;
                        SFX_3D_Player[i].outputAudioMixerGroup = SFX;
                        SFX_3D_Player[i].Play();
                        StartCoroutine(ResetAudioSourceParent(SFX_3D_Player[i], s.clip.length));
                        return;
                    }
                }
                Debug.Log("모든 3D SFX 슬롯이 사용 중입니다.");
                return;
            }
        }
        Debug.Log($"해당 name:[{name}] key를 가진 3DSFX가 없습니다.");
    }

    private IEnumerator ResetAudioSourceParent(AudioSource source, float delay)
    {
        float timer = 0f;
        while (timer < delay || source.isPlaying)
        {
            timer += Time.deltaTime;

            // 재생 도중 부모(몬스터 등)가 사라졌는지 체크
            if (source.transform.parent == null || source.transform.parent.gameObject.activeInHierarchy == false)
            {
                // 부모가 사라졌다면 즉시 원래 컨테이너로 복귀시켜서 미아(분실) 방지
                source.transform.SetParent(sfx3DContainer);
                yield break;
            }
            yield return null;
        }

        // 재생 완료 후 복귀
        if (source != null)
        {
            source.transform.SetParent(sfx3DContainer);
            source.transform.localPosition = Vector3.zero; // 위치 초기화
        }
    }
}
