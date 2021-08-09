using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class FadeController : MonoBehaviour
{
    public enum FadeInReserveType
    {
        None,
        FadeIn,
        InstantFadeIn
    }

    [SerializeField]
    Image image;

    [SerializeField]
    float initialFadeInTime;

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

    public void FadeOut(FadeInReserveType fadeInReserveType = FadeInReserveType.None)
    {
        isFadeIn = false;
        isFadeOut = true;

        switch(fadeInReserveType)
        {
            case FadeInReserveType.FadeIn:
                onFadeOutComplete.AddListener(FadeIn);
                break;

            case FadeInReserveType.InstantFadeIn:
                onFadeOutComplete.AddListener(InstantFadeIn);
                break;
        }
    }

    public void InstantFadeOut()
    {
        isFadeIn = false;
        isFadeOut = false;
        color = Color.black;
        image.color = color;
    }

    void FadeIn()
    {
        isFadeIn = true;
        isFadeOut = false;
        isWaitingFadeOutEvent = false;

        onFadeOutComplete.RemoveAllListeners();
    }

    void InstantFadeIn()
    {
        onFadeOutComplete.RemoveAllListeners();

        color.a = 0;
        image.color = color;
        
        if(invokeOnFadeInEvents == true)
        {
            onFadeInComplete.Invoke();
            invokeOnFadeInEvents = false;
        }
    }

    IEnumerator InitialFadeIn()
    {
        yield return new WaitForSeconds(initialFadeInTime);

        isFadeIn = true;
        isFadeOut = false;
        isWaitingFadeOutEvent = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        fadeInReciprocal = 1 / fadeInTime;
        fadeOutReciprocal = 1 / fadeOutTime;

        color = Color.black;
        image.color = color;

        invokeOnFadeInEvents = true;
        fadeOutEventWaitingTimer = fadeOutEventWaitingTime;

        StartCoroutine(InitialFadeIn());
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
