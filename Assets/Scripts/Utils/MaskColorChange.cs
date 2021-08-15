using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MaskColorChange : MonoBehaviour
{
    RawImage[] rawImageList;
    Image[] imageList;
    TextMeshProUGUI[] textList;

    void Awake()
    {
        rawImageList = transform.GetComponentsInChildren<RawImage>();
        imageList = transform.GetComponentsInChildren<Image>();
        textList = transform.GetComponentsInChildren<TextMeshProUGUI>();
    }
    
    void Start()
    {
        GameManager.UIController.AddMaskColorChange(this);
    }

    public void ChangeComponentColor(Color color)
    {
        foreach(RawImage rawImage in rawImageList)
        {
            rawImage.materialForRendering.color = color;
        }
        foreach(Image image in imageList)
        {
            image.materialForRendering.color = color;
        }
        foreach(TextMeshProUGUI textList in textList)
        {
            textList.materialForRendering.SetColor("_FaceColor", color);
        }
    }
}
