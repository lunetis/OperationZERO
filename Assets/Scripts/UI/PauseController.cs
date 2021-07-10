using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class PauseController : MonoBehaviour
{
    [SerializeField]
    List<RectTransform> selectableOptions;
    [SerializeField]
    RectTransform selectIndicator;

    [SerializeField]
    TextMeshProUGUI descriptionText;

    [SerializeField]
    AudioSource audioSource;
    
    [SerializeField]
    AudioClip pauseAudioClip;
    [SerializeField]
    AudioClip scrollAudioClip;
    [SerializeField]
    AudioClip confirmAudioClip;

    [SerializeField]
    bool playUIShowAudio;

    int currentIndex;

    UISelect GetCurrentUISelect()
    {
        return selectableOptions[currentIndex].GetComponent<UISelect>();
    }

    void ChangeSelection()
    {
        // Cursor
        selectIndicator.anchoredPosition = new Vector2(selectIndicator.anchoredPosition.x,
                                                       selectableOptions[currentIndex].anchoredPosition.y);
        // Description
        descriptionText.text = GetCurrentUISelect()?.Description;
    }

    public void Navigate(InputAction.CallbackContext context)
    {
        float y = context.ReadValue<Vector2>().y;

        if(y == -1 && currentIndex < selectableOptions.Count - 1)
        {
            ++currentIndex;
        }
        else if(y == 1 && currentIndex > 0)
        {
            --currentIndex;
        }
        else return;
                                                    
        audioSource.PlayOneShot(scrollAudioClip);

        ChangeSelection();
    }

    public void Confirm(InputAction.CallbackContext context)
    {
        audioSource.PlayOneShot(confirmAudioClip);

        GetCurrentUISelect()?.OnSelectEvent.Invoke();
    }

    void Awake()
    {
        audioSource.ignoreListenerPause = true;
    }

    void OnEnable()
    {
        if(GameManager.PlayerInput == null) return;
        if(playUIShowAudio == true) audioSource.PlayOneShot(pauseAudioClip);

        currentIndex = 0;
        ChangeSelection();

        InputAction navigateAction = GameManager.PlayerInput.actions.FindAction("Navigate");
        navigateAction.started += Navigate;
        InputAction submitAction = GameManager.PlayerInput.actions.FindAction("Submit");
        submitAction.started += Confirm;
    }

    void OnDisable()
    {
        if(GameManager.PlayerInput == null) return;
        audioSource.PlayOneShot(pauseAudioClip);

        InputAction navigateAction = GameManager.PlayerInput.actions.FindAction("Navigate");
        navigateAction.started -= Navigate;
        InputAction submitAction = GameManager.PlayerInput.actions.FindAction("Submit");
        submitAction.started -= Confirm;
    }
}