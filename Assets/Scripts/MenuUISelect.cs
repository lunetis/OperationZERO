using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class MenuUISelect : UISelect
{
    Animation anim;
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    Image image;

    void Awake()
    {
        anim = GetComponent<Animation>();
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        image.rectTransform.sizeDelta = textMeshPro.GetPreferredValues();
        anim.Play();
    }
}
