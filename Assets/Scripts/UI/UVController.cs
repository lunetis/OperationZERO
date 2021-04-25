using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UVController : MonoBehaviour
{
    public enum ChangeAxisType
    {
        U,
        V
    }

    public ChangeAxisType axisType = ChangeAxisType.U;
    private RawImage image;
    public float unitValue;
    public float imageUnitCnt;

    float reciprocal;

    void Awake()
    {
        image = GetComponent<RawImage>();
        reciprocal = 1 / unitValue / imageUnitCnt;
    }

    public void SetUV(float value)
    {
        // range[0 - unitValue] -> [0 - 1 / imageUnitCnt]
        float remainder = value % unitValue;

        if(axisType == ChangeAxisType.U)
        {
            image.uvRect = new Rect(remainder * reciprocal, 0, 1, 1);
        }
        else if(axisType == ChangeAxisType.V)
        {
            image.uvRect = new Rect(0, remainder * reciprocal, 1, 1);
        }
    }
}