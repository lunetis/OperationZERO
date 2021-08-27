using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioController : MonoBehaviour
{
    public const float MIN_VOLUME = -80;
    public const float MAX_VOLUME = 0;

    [Header("BGM")]
    [SerializeField]
    AudioClip introBGM;
    [SerializeField]
    AudioClip checkpointIntroBGM;
    [SerializeField]
    AudioClip loopBGM;
    [SerializeField]
    float bgmPlayDelay = 1;

    [Header("Audio Control")]
    [SerializeField]
    AudioMixer audioMixer;
    
    [SerializeField]
    AudioSource bgmIntroAudioSource;
    [SerializeField]
    AudioSource bgmLoopAudioSource;

    [SerializeField]
    float cutsceneBGMVolume = -7;
    
    [SerializeField]
    float volumeLerpAmount = 0.5f;

    float bgmVolume;
    float cutsceneVolume;
    float sfxVolume;

    float targetBGMVolume;
    float targetCutsceneVolume;
    float targetSFXVolume;

    double nextEventTime;

    public float TargetBGMVolume
    {
        set { targetBGMVolume = value; }
    }
    public float TargetCutsceneVolume
    {
        set { targetCutsceneVolume = value; }
    }
    public float TargetSFXVolume
    {
        set { targetSFXVolume = value; }
    }
    
    public void OnCutsceneStart(bool muteBGM = false)
    {
        targetBGMVolume = (muteBGM == true) ? MIN_VOLUME : cutsceneBGMVolume;
        targetCutsceneVolume = MAX_VOLUME;
        cutsceneVolume = MAX_VOLUME;
        audioMixer.SetFloat("SFXVolume", MIN_VOLUME);
        audioMixer.SetFloat("CutsceneVolume", MAX_VOLUME);
    }

    public void OnCutsceneFadeOut()
    {
        targetCutsceneVolume = MIN_VOLUME;
    }

    public void OnCutsceneEnd()
    {
        targetBGMVolume = MAX_VOLUME;
        audioMixer.SetFloat("SFXVolume", MAX_VOLUME);
    }

    void PlayLoopBGM()
    {
        bgmLoopAudioSource.clip = loopBGM;
        bgmLoopAudioSource.Play();
    }

    void Awake()
    {
        targetBGMVolume = bgmVolume = 0;
        targetCutsceneVolume = cutsceneVolume = 0;
        targetSFXVolume = sfxVolume = 0;
        
        audioMixer.SetFloat("BGMVolume", bgmVolume);
        audioMixer.SetFloat("SFXVolume", MAX_VOLUME);
        audioMixer.SetFloat("CutsceneVolume", cutsceneVolume);
    }

    void Start()
    {
        if(introBGM == null || loopBGM == null)
        {
            enabled = false;
            return;
        }

        AudioClip introAudioClip = introBGM;

        if(MissionManager.phase > 1 && checkpointIntroBGM != null)
        {
            introAudioClip = checkpointIntroBGM;
            bgmPlayDelay = 0.2f;
        }
        bgmIntroAudioSource.clip = introAudioClip;
        bgmIntroAudioSource.loop = false;
        bgmIntroAudioSource.PlayScheduled(AudioSettings.dspTime + bgmPlayDelay);

        nextEventTime = AudioSettings.dspTime + bgmPlayDelay + introAudioClip.length;
        bgmLoopAudioSource.clip = loopBGM;
        
        bgmLoopAudioSource.loop = true;
        bgmLoopAudioSource.PlayScheduled(nextEventTime);
        
        #if UNITY_WEBGL
        bgmLoopAudioSource.loop = false;
        #else
        bgmLoopAudioSource.loop = true;
        #endif
    }

    void CheckWebGLAudioLoop()
    {
        if(bgmLoopAudioSource.isPlaying == false)
        {
            bgmLoopAudioSource.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // BGM
        bgmVolume = Mathf.Lerp(bgmVolume, targetBGMVolume, Time.deltaTime);
        audioMixer.SetFloat("BGMVolume", bgmVolume);

        // Cutscene
        cutsceneVolume = Mathf.Lerp(cutsceneVolume, targetCutsceneVolume, Time.deltaTime * volumeLerpAmount);
        audioMixer.SetFloat("CutsceneVolume", cutsceneVolume);

        // WebGL
        // Issue: When playing audio with PlayScheduled(), it doesn't loop whether the loop property is checked or not.
        #if UNITY_WEBGL
        CheckWebGLAudioLoop();
        #endif
    }
}
