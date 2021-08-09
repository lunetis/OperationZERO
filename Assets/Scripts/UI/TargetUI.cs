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

    bool isInvisible;

    public bool IsInvisible
    {
        set { isInvisible = value; }
    }

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

    public GameObject UIObject
    {
        get { return uiObject; }
    }

    Vector2 screenSize;
    float screenAdjustFactor;
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
            CancelInvoke();
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
    void OnDestroy()
    {
        targetObject = null;
        CancelInvoke();
    }


    void Start()
    {
        isInvisible = true;
        rectTransform = GetComponent<RectTransform>();

        screenSize = new Vector2(Screen.width, Screen.height);
        screenAdjustFactor = Mathf.Max((1920.0f / Screen.width), (1080.0f / Screen.height));
        Target = targetObject;  // execute Setter code
    }

    // Update is called once per frame
    void Update()
    {
        activeCamera = GameManager.CameraController.GetActiveCamera();

        if(targetObject == null && activeCamera == null)
            return;

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
            Vector2 position = screenPoint - screenSize * 0.5f;
            position *= screenAdjustFactor;
            rectTransform.anchoredPosition = position;
        }

        // the transform is outside of camera view (not behind, we need to consider Field of View)
        bool isOutsideOfCamera = (screenPosition.z < 0 || 
                            screenPosition.x < 0 || screenPosition.x > screenSize.x || 
                            screenPosition.y < 0 || screenPosition.y > screenSize.y);


        uiObject.SetActive(isOutsideOfCamera == false && isInvisible == true && distance < hideDistance);

        GameManager.TargetController.ShowTargetArrow(isOutsideOfCamera && distance < hideDistance);
    }
}
