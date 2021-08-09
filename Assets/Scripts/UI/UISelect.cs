using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelect : MonoBehaviour
{
    [SerializeField]
    string descriptionKey;
    [SerializeField]
    UnityEvent onSelectEvent;
    
    public string DescriptionKey
    {
        get { return descriptionKey; }
    }

    public UnityEvent OnSelectEvent
    {
        get { return onSelectEvent; }
    }
}
