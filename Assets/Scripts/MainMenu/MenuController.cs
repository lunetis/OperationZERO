using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class MenuController : MonoBehaviour
{
    [SerializeField]
    List<RectTransform> selectableOptions;
    [SerializeField]
    RectTransform selectIndicator;
    
    [SerializeField]
    UnityEvent onNavigateEvent;

    [SerializeField]
    UnityEvent onBackEvent;

    [SerializeField]
    TextMeshProUGUI descriptionText;

    [SerializeField]
    AudioSource audioSource;

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
        // descriptionText.text = GetCurrentUISelect()?.Description;
    }

    IEnumerator ConfirmCoroutine()
    {
        MainMenuController.PlayerInput.enabled = false;
        yield return new WaitForSeconds(0.3f);
        MainMenuController.PlayerInput.enabled = true;
        GetCurrentUISelect()?.OnSelectEvent.Invoke();
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
        
        onNavigateEvent.Invoke();
        ChangeSelection();
    }

    public void Confirm(InputAction.CallbackContext context)
    {
        if(selectIndicator != null)
        {
            selectIndicator.GetComponent<Animation>().Play();
        }
        StartCoroutine(ConfirmCoroutine());
    }
    
    public void Back(InputAction.CallbackContext context)
    {
        onBackEvent.Invoke();
    }

    void OnEnable()
    {
        currentIndex = 0;
        ChangeSelection();
    }
}