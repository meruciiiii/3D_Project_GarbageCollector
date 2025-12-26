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
    [SerializeField] private AudioMixer audioMixer;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            AutoSetting();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        LoadAllVolumes();
    }
    private void AutoSetting()
    {
        BGM_Player = transform.GetChild(0).GetComponent<AudioSource>();
        SFX_Player = transform.GetChild(1).GetComponents<AudioSource>();
        sfx3DContainer = transform.GetChild(2);
        SFX_3D_Player = transform.GetChild(2).GetComponentsInChildren<AudioSource>();
    }

    private void LoadAllVolumes()
    {
        SetVolume("Master", PlayerPrefs.GetFloat("volume_Master", defaultVolume));
        SetVolume("BGM", PlayerPrefs.GetFloat("volume_BGM", defaultVolume));
        SetVolume("SFX", PlayerPrefs.GetFloat("volume_SFX", defaultVolume));
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
    private const float defaultVolume = 75f;

    public void SetVolume(string parameter, float sliderValue)
    {
        // 1. 오디오 믹서 적용
        float normalizeValue = Mathf.Clamp(sliderValue / 100f, 0.0001f, 1f);
        float dB = Mathf.Log10(normalizeValue) * 20;
        audioMixer.SetFloat(parameter, dB);

        // 2. 데이터 저장
        PlayerPrefs.SetFloat("volume_" + parameter, sliderValue);
    }

    // 현재 저장된 볼륨 값을 가져오는 함수 (슬라이더 초기화용)
    public float GetSavedVolume(string parameter)
    {
        return PlayerPrefs.GetFloat("volume_" + parameter, 75f);
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
        if (BGM_Player.isPlaying) // 1. Fade Out
        {
            float startVol = BGM_Player.volume; // 현재 볼륨 기억
            while (BGM_Player.volume > 0)
            {
                // 그냥 1/fadeDuration을 빼면, 현재 볼륨이 0.5일 때 예상보다 빨리 꺼질 수 있음
                // startVol을 곱해주는 것이 더 자연스러운 곡선을 만듭니다.
                BGM_Player.volume -= startVol * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        BGM_Player.volume = 0;
        BGM_Player.Stop();
        BGM_Player.clip = newClip; // 2. 클립 교체
        BGM_Player.outputAudioMixerGroup = BGM;
        BGM_Player.loop = true;
        BGM_Player.Play();
        
        while (BGM_Player.volume < 1.0f) // 3. Fade In
        {
            BGM_Player.volume += Time.deltaTime / fadeDuration;
            yield return null;
        }
        BGM_Player.volume = 1.0f;
    }

    public void StopBGM()
    {
        // 1. 이미 페이드 로직이 돌아가고 있다면 중지
        if (bgmFadeCoroutine != null)
            StopCoroutine(bgmFadeCoroutine);

        // 2. 부드럽게 꺼지는 페이드 아웃 코루틴 시작
        bgmFadeCoroutine = StartCoroutine(FadeOutAndStopBGM());
    }

    private IEnumerator FadeOutAndStopBGM()
    {
        if (BGM_Player.isPlaying)
        {
            float startVolume = BGM_Player.volume;

            // 현재 볼륨에서 0까지 fadeDuration에 걸쳐 감소
            while (BGM_Player.volume > 0)
            {
                BGM_Player.volume -= startVolume * Time.deltaTime / fadeDuration;
                yield return null;
            }
        }

        // 완전히 멈춤 및 초기화
        BGM_Player.Stop();
        BGM_Player.clip = null;
        BGM_Player.volume = 1.0f; // 다음 재생을 위해 볼륨만 기본값으로 복구
        bgmFadeCoroutine = null;
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
    
    public void Play3DSFX(string name, Transform gameobject) 
        //사용방식 AudioManager.instance.Play3DSFX("3D_SFX", this.transform);
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
        // 1. 소리가 시작될 때의 부모를 기억합니다.
        Transform currentTarget = source.transform.parent;

        while (timer < delay || source.isPlaying)
        {
            timer += Time.deltaTime;

            // 2. 부모가 살아있고 활성화된 상태인지 실시간 체크
            if (currentTarget != null && currentTarget.gameObject.activeInHierarchy)
            {
                // 부모가 이동 중일 수 있으므로 위치를 계속 동기화합니다.
                // (자식으로 되어있다면 자동으로 따라가지만, 안전을 위해 체크만 합니다.)
            }
            else
            {
                // 3. 부모가 사라진(Destroy/Disable) 순간!
                // 독립시키기 직전의 위치를 마지막으로 고정합니다.
                if (source.transform.parent != sfx3DContainer)
                {
                    Vector3 lastPos = source.transform.position; // 마지막 위치 기억
                    source.transform.SetParent(null);// 월드 좌표로 분리 (이제 안 따라감)
                    source.transform.position = lastPos; // 위치 고정
                }

                // 4. 이제 부모가 없으므로 루프를 빠져나가 소리만 끝날 때까지 대기합니다.
                yield return new WaitWhile(() => source != null && source.isPlaying);
                break;
            }

            yield return null;
        }

        // 5. 모든 상황 종료 후 회수
        if (source != null)
        {
            source.Stop();
            source.transform.SetParent(sfx3DContainer);
            source.transform.localPosition = Vector3.zero;
        }
    }
}
