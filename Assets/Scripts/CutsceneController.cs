using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class CutsceneController : MonoBehaviour
{
    [SerializeField]
    PlayerInput playerInput;

    [Header("Cutscene Properties")]
    [SerializeField]
    PlayableAsset phase3CutsceneAsset;
    [SerializeField]
    PlayableAsset endingCutsceneAsset;

    [SerializeField]
    UnityEvent onCutsceneStart;
    [SerializeField]
    UnityEvent onPhase3CutsceneEnded;
    [SerializeField]
    UnityEvent onEndingCutsceneEnded;

    [SerializeField]
    GameObject cutsceneCamera;
    [SerializeField]
    PlayableDirector playableDirector;

    [Header("Skip Properties")]
    [SerializeField]
    GameObject skipUI;
    [SerializeField]
    FadeController fadeController;

    [SerializeField]
    float skipCheckTime = 3.0f;

    [SerializeField]
    AudioController audioController;

    // ======= Input =======

    public void OnSkip(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            // First Press
            if(skipUI.activeInHierarchy == false)
            {
                skipUI.SetActive(true);
                Invoke("HideSkipUI", skipCheckTime);
            }
            // Second Press
            else
            {
                Skip();
                CancelInvoke("HideSkipUI");
            }
        }
    }

    void HideSkipUI()
    {
        skipUI.SetActive(false);
    }

    void Skip()
    {
        audioController.OnCutsceneFadeOut();
        fadeController.OnFadeOutComplete.AddListener(playableDirector.Stop);
        fadeController.FadeOut(FadeController.FadeInReserveType.InstantFadeIn);
    }
    
    // ======= Common =======

    void OnCutsceneStart()
    {
        if(GameManager.Instance.IsGameOver == true) return;
        
        audioController.OnCutsceneStart();
        playerInput.SwitchCurrentActionMap("Cutscene");

        GameManager.ScriptManager.ClearScriptQueue();   // In case of remained script exists
        onCutsceneStart.Invoke();
        GameManager.CameraController.GetActiveCamera().GetComponent<AudioListener>().enabled = false;
        cutsceneCamera.SetActive(true);
    }

    // ======= Phase 3 =======

    public void PlayPhase3Cutscene()
    {
        OnCutsceneStart();
        playableDirector.playableAsset = phase3CutsceneAsset;
        playableDirector.stopped += OnPhase3CutsceneEnded;
        playableDirector.Play();
    }

    void OnPhase3CutsceneEnded(PlayableDirector director)
    {
        audioController.OnCutsceneEnd();
        onPhase3CutsceneEnded.Invoke();
        cutsceneCamera.SetActive(false);
        playableDirector.stopped -= OnPhase3CutsceneEnded;
        GameManager.CameraController.GetActiveCamera().GetComponent<AudioListener>().enabled = true;
        
        playerInput.SwitchCurrentActionMap("Player");
        skipUI.SetActive(false);
    }

    // ======= Ending =======
    
    public void PlayEndingCutscene()
    {
        OnCutsceneStart();
        playableDirector.playableAsset = phase3CutsceneAsset;
        playableDirector.stopped += OnEndingCutsceneEnded;
        playableDirector.Play();
    }

    void OnEndingCutsceneEnded(PlayableDirector director)
    {
        audioController.OnCutsceneEnd();
        onEndingCutsceneEnded.Invoke();
        cutsceneCamera.SetActive(false);
        playableDirector.stopped -= OnEndingCutsceneEnded;

        playerInput.SwitchCurrentActionMap("Player");
        skipUI.SetActive(false);
    }

    void Awake()
    {
        skipUI.SetActive(false);
    }
}
