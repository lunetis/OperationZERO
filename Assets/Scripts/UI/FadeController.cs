using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeController : MonoBehaviour
{
    [SerializeField]
    Image image;

    [SerializeField]
    float fadeInTime;
    [SerializeField]
    float fadeOutTime;
    
    [SerializeField]
    float fadeOutEventWaitingTime;
    float fadeOutEventWaitingTimer;
    
    float fadeInReciprocal;
    float fadeOutReciprocal;

    Color color;

    bool isFadeOut;
    bool isFadeIn;
    bool isWaitingFadeOutEvent;

    bool invokeOnFadeInEvents;
    
    // Events
    [SerializeField]
    UnityEvent onFadeInComplete;
    
    [SerializeField]
    UnityEvent onFadeOutComplete;

    public UnityEvent OnFadeInComplete
    {
        get { return onFadeInComplete; }
        set { onFadeInComplete = value; }
    }

    public UnityEvent OnFadeOutComplete
    {
        get { return onFadeOutComplete; }
        set { onFadeOutComplete = value; }
    }

    public void FadeOut(bool reserveFadeIn = false)
    {
        isFadeIn = false;
        isFadeOut = true;

        if(reserveFadeIn == true)
        {
            onFadeOutComplete.AddListener(FadeIn);
        }
    }

    void FadeIn()
    {
        isFadeIn = true;
        isFadeOut = false;
        isWaitingFadeOutEvent = false;

        onFadeOutComplete.RemoveAllListeners();
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeInReciprocal = 1 / fadeInTime;
        fadeOutReciprocal = 1 / fadeOutTime;

        isFadeIn = true;
        isFadeOut = false;
        isWaitingFadeOutEvent = false;

        color = Color.black;
        image.color = color;

        invokeOnFadeInEvents = true;
        fadeOutEventWaitingTimer = fadeOutEventWaitingTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(isFadeIn == true)
        {
            color.a -= Time.unscaledDeltaTime * fadeInReciprocal;
            image.color = color;
            
            if(color.a <= 0)
            {
                color.a = 0;
                isFadeIn = false;

                if(invokeOnFadeInEvents == true)
                {
                    onFadeInComplete.Invoke();
                    invokeOnFadeInEvents = false;
                }
            }
        }

        if(isFadeOut == true)
        {
            color.a += Time.unscaledDeltaTime * fadeOutReciprocal;
            image.color = color;

            if(color.a >= 1)
            {
                color.a = 1;
                isFadeOut = false;
                isWaitingFadeOutEvent = true;
            }
        }

        if(isWaitingFadeOutEvent == true)
        {
            fadeOutEventWaitingTimer -= Time.unscaledDeltaTime;
            if(fadeOutEventWaitingTimer <= 0)
            {
                fadeOutEventWaitingTimer = fadeOutEventWaitingTime;
                isWaitingFadeOutEvent = false;
                onFadeOutComplete.Invoke();
            }
        }
    }
}
