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
            InitializeDictionary();
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

        walkingPlayer = gameObject.AddComponent<AudioSource>();
        walkingPlayer.playOnAwake = false;
        walkingPlayer.loop = true; // 기본적으로 루프 활성
        walkingPlayer.outputAudioMixerGroup = SFX; // SFX 믹서 그룹 할당
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
    private AudioSource walkingPlayer; // 걷기 전용 플레이어

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
        if (BGM_Player.clip == null)
        {
            BGM_Player.clip = newClip;
            BGM_Player.outputAudioMixerGroup = BGM;
            BGM_Player.loop = true;
            BGM_Player.volume = 1.0f; // 볼륨을 즉시 1로 설정
            BGM_Player.Play();
            bgmFadeCoroutine = null;
            yield break; // 코루틴 종료
        }

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
        bgmFadeCoroutine = null; 
        // 코루틴 참조 해제 다음번에 PlayBGM이 호출될 때 불필요한 StopCoroutine이 발생하지 않도록
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


    private Dictionary<string, AudioClip> sfxDictionary = new Dictionary<string, AudioClip>();

    private void InitializeDictionary()
    {
        foreach (Sound s in SFX_clip)
        {
            if (!sfxDictionary.ContainsKey(s.name))
                sfxDictionary.Add(s.name, s.clip);
        }
    }
    public void PlaySFX(string name)
    {
        //foreach(Sound s in SFX_clip)
        //{
        //    if (s.name.Equals(name))
        //    {
        //        for (int i = 0; i < SFX_Player.Length; i++)
        //        {
        //            if (!SFX_Player[i].isPlaying)
        //            {
        //                SFX_Player[i].clip = s.clip;
        //                SFX_Player[i].outputAudioMixerGroup = SFX;
        //                SFX_Player[i].Play();
        //                return;
        //            }
        //        }
        //        Debug.Log("모든 Audio Source SFXPlayer가 Play중");
        //        return;
        //    }
        //}

         //리스트를 도는 대신 딕셔너리에서 바로 찾음
        if (sfxDictionary.TryGetValue(name, out AudioClip clip))
        {
            for (int i = 0; i < SFX_Player.Length; i++)
            {
                if (!SFX_Player[i].isPlaying)
                {
                    SFX_Player[i].clip = clip;
                    SFX_Player[i].outputAudioMixerGroup = SFX;
                    SFX_Player[i].Play();
                    return;
                }
            }
        }
    }
    
    public void Play3DSFX(string name, Transform gameobject, bool isLoop = false) 
        //사용방식 AudioManager.instance.Play3DSFX("3D_SFX", this.transform);
    {
        if (gameobject == null) return; // 타겟이 없으면 실행 안 함

        foreach (Sound s in SFX_3D_clip)
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
                        SFX_3D_Player[i].loop = isLoop; // 루프 설정
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

    public void Stop3DSFX(string name, Transform targetTransform)
    {
        foreach (var player in SFX_3D_Player)
        {
            // 1. 현재 재생 중이고
            // 2. 클립 이름이 일치하며
            // 3. 부모(Transform)가 요청한 오브젝트와 일치하는 경우만 중지
            if (player.isPlaying && player.clip != null &&
                player.clip.name == name && player.transform.parent == targetTransform)
            {
                player.Stop();
                player.loop = false;
                // 이후 처리는 ResetAudioSourceParent 코루틴이 담당
            }
        }
    }

    private IEnumerator ResetAudioSourceParent(AudioSource source, float delay)
    {
        float timer = 0f;
        // 1. 소리가 시작될 때의 부모를 기억합니다.
        Transform currentTarget = source.transform.parent;

        // 소리가 끝날 때까지 또는 설정된 clip 길이 동안 체크
        while (source.loop || timer < delay || source.isPlaying)
        {
            timer += Time.deltaTime;

            // 2. 원래 부모가 사라졌거나(Destroy) 비활성화(Disable) 되었는지 체크
            if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            {
                // 부모가 사라진 순간!
                if (source.transform.parent != sfx3DContainer)
                {
                    // 현재 월드 좌표를 유지하기 위해 위치를 기록
                    Vector3 currentWorldPos = source.transform.position;

                    // 3. 즉시 매니저의 컨테이너로 부모 변경 (좌표 유지를 위해 worldPositionStays: true)
                    source.transform.SetParent(sfx3DContainer, true);

                    // 만약 worldPositionStays가 불안정할 경우를 대비해 수동 재설정
                    source.transform.position = currentWorldPos;
                }

                if (source.loop)
                {
                    source.loop = false;
                    source.Stop();
                    break;
                }

                // 4. 이제 독립되었으므로 소리가 끝날 때까지만 대기하고 루프 종료
                yield return new WaitWhile(() => source != null && source.isPlaying);
                break;
            }

            // Stop3DSFX 등으로 루프가 풀리고 소리가 멈췄다면 루프 탈출
            if (!source.loop && !source.isPlaying && timer > 0.1f) break;

            yield return null;
        }

        // 5. 모든 상황 종료 후 (소리 끝) 리셋 및 회수
        if (source != null)
        {
            source.Stop();
            source.clip = null; // 참조 해제
            source.loop = false;
            source.transform.SetParent(sfx3DContainer);
            source.transform.localPosition = Vector3.zero; // 다음 사용을 위해 위치 초기화
        }
    }

    // 걷기 사운드 시작
    public void PlayWalkingSound(string name)
    {
        // walkingPlayer.clip이 null일 경우를 대비해 순서대로 체크
        if (walkingPlayer.isPlaying && walkingPlayer.clip != null)
        {
            if (walkingPlayer.clip.name == name) return;
        }

        foreach (Sound s in SFX_clip)
        {
            if (s.name.Equals(name))
            {
                walkingPlayer.clip = s.clip;
                walkingPlayer.Play();
                return;
            }
        }
    }

    // 걷기 사운드 중지
    public void StopWalkingSound()
    {
        if (walkingPlayer.isPlaying)
        {
            walkingPlayer.Stop();
        }
    }
}
