using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class LocalizedText : MonoBehaviour
{
    TextMeshProUGUI textMeshPro;
    [SerializeField]
    string subtitleKey;
    
    // Start is called before the first frame update
    void Awake()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    void Start()
    {
        textMeshPro.text = GameManager.ScriptManager.GetSubtitleText(subtitleKey);
    }
}
