using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TargetUI : MonoBehaviour
{
    TargetObject targetObject;

    [Header("UI / Texts")]
    [SerializeField]
    RawImage frameImage;

    [SerializeField]
    TextMeshProUGUI distanceText;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI nicknameText;
    [SerializeField]
    TextMeshProUGUI targetText;

    [Header("Properties")]
    [SerializeField]
    bool isMainTarget;
    
    [SerializeField]
    float hideDistance;
    
    [SerializeField]
    GameObject uiObject;
    [SerializeField]
    GameObject blinkUIObject;
    [SerializeField]
    GameObject nextTargetText;
    
    [SerializeField]
    float blinkRepeatTime;

    bool isTargetted;
    bool isNextTarget;
    bool isBlinking;

    ObjectInfo objectInfo;
    RectTransform rectTransform;

    public TargetObject Target
    {
        get
        {
            return targetObject;
        }

        set
        {
            targetObject = value;
            objectInfo = targetObject.Info;

            nameText.text = objectInfo.ObjectName;
            nicknameText.text = objectInfo.ObjectNickname;
            targetText.gameObject.SetActive(objectInfo.MainTarget);
        }
    }

    RectTransform canvasRect;
    Camera activeCamera;
    
    // Recursive search
    Canvas GetCanvas(Transform parentTransform)
    {
        if(parentTransform.GetComponent<Canvas>() != null)
        {
            return parentTransform.GetComponent<Canvas>();
        }
        else
        {
            return GetCanvas(parentTransform.parent);
        }
    }

    public void SetTargetted(bool isTargetted)
    {
        this.isTargetted = isTargetted;
        SetBlink(isTargetted);
        frameImage.color = GameManager.NormalColor;
    }

    void SetBlink(bool blink)
    {
        if(isBlinking == blink) return;

        if(blink == true)
        {
            isBlinking = true;
            InvokeRepeating("Blink", 0, blinkRepeatTime);
        }
        else
        {
            isBlinking = false;
            CancelInvoke("Blink");
            blinkUIObject.SetActive(true);
        }
    }

    void Blink()
    {
        blinkUIObject.SetActive(!blinkUIObject.activeInHierarchy);
    }

    public void SetLock(bool isLocked)
    {
        if(isLocked == true)
        { 
            SetBlink(false);
            frameImage.color = GameManager.WarningColor;
        }
        else
        {
            SetTargetted(targetObject != null);
            frameImage.color = GameManager.NormalColor;
        }
    }

    // Call before destroy
    public void DestroyUI()
    {
        targetObject = null;
        CancelInvoke();
    }


    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        Canvas canvas = GetCanvas(transform.parent);
        if(canvas != null)
        {
            canvasRect = canvas.GetComponent<RectTransform>();
        }

        Target = targetObject;  // execute Setter code
    }

    // Update is called once per frame
    void Update()
    {
        if(targetObject == null)
            return;

        activeCamera = GameManager.CameraController.GetActiveCamera();
        Vector3 screenPosition = activeCamera.WorldToScreenPoint(targetObject.transform.position);
        float distance = GameManager.Instance.GetDistanceFromPlayer(targetObject.transform);
        nextTargetText.SetActive(targetObject.isNextTarget);

        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // Text
            distanceText.text = string.Format("{0:0}", distance);
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(activeCamera, targetObject.transform.position);
            rectTransform.anchoredPosition = screenPoint - canvasRect.sizeDelta * 0.5f;
        }

        // the transform is outside of camera view (not behind, we need to consider Field of View)
        bool isOutsideOfCamera = (screenPosition.z < 0 || 
                            screenPosition.x < 0 || screenPosition.x > canvasRect.sizeDelta.x || 
                            screenPosition.y < 0 || screenPosition.y > canvasRect.sizeDelta.y);


        uiObject.SetActive(distance < hideDistance);
        GameManager.TargetController.ShowTargetArrow(isOutsideOfCamera && distance < hideDistance);
    }
}
