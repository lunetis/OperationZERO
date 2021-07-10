using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UISelect : MonoBehaviour
{
    [SerializeField]
    string description;
    [SerializeField]
    UnityEvent onSelectEvent;
    
    public string Description
    {
        get { return description; }
    }

    public UnityEvent OnSelectEvent
    {
        get { return onSelectEvent; }
    }
}
