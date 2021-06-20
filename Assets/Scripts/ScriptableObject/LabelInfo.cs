using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LabelInfo", menuName = "Scriptable Object Asset/LabelInfo")]
public class LabelInfo : ScriptableObject
{
    [SerializeField]
    Texture labelTexture;
    [SerializeField]
    Color labelColor;
    [SerializeField]
    float visibleTime;
    
    [SerializeField]
    AudioClip audioClip;

    public Texture LabelTexture
    {
        get { return labelTexture; }
    }
    
    public Color LabelColor
    {
        get { return labelColor; }
    }

    public float VisibleTime
    {
        get { return visibleTime; }
    }

    public AudioClip AudioClip
    {
        get { return audioClip; }
    }
}
