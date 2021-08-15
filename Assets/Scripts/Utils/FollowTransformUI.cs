using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransformUI : MonoBehaviour
{
    [SerializeField]
    protected Transform targetTransform;

    [SerializeField]
    protected Camera cam;
    protected RectTransform rectTransform;
    protected RectTransform canvasRect;

    [SerializeField]
    protected bool trackCurrentCamera = true;

    protected Vector2 screenSize;
    protected float screenAdjustFactor;

    [SerializeField]
    public bool preventBlinkError = false;

    [SerializeField]
    [Range(0.01f, 1)]
    float errorRange = 0.1f;
    Vector2 initPos;
    Vector2 prevPos;
    float errorDistanceThreshold;

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

    // Start is called before the first frame update
    protected virtual void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        canvasRect = GetCanvas(transform).GetComponent<RectTransform>();
        screenSize = new Vector2(Screen.width, Screen.height);
        screenAdjustFactor = Mathf.Max((1920.0f / Screen.width), (1080.0f / Screen.height));

        // Error Check
        initPos = rectTransform.anchoredPosition;
        errorDistanceThreshold = screenSize.x * errorRange;
        prevPos = Vector2.zero;
    }

    float GetXYSum(Vector2 vec)
    {
        return vec.x + vec.y;
    }

    void CheckError(Vector2 pos)
    {
        // Was it close enough to initial position;
        if(prevPos == Vector2.zero)
        {
            if((initPos - pos).magnitude < errorDistanceThreshold)
            {
                rectTransform.anchoredPosition = pos;
                prevPos = pos;
            }
        }
        else
        {
            if((prevPos - pos).magnitude < errorDistanceThreshold * 0.5f)
            {
                rectTransform.anchoredPosition = pos;
                prevPos = pos;
            }
            else
            {
                prevPos = Vector2.zero;
            }
        }
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if(targetTransform == null) return;
        
        if(trackCurrentCamera == true)
        {
            cam = GameManager.CameraController.GetActiveCamera();
        }

        Vector3 screenPosition = cam.WorldToScreenPoint(targetTransform.position);
        float distance = GameManager.Instance.GetDistanceFromPlayer(targetTransform);

        // if screenPosition.z < 0, the object is behind camera
        if(screenPosition.z > 0)
        {
            // UI Position
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(cam, targetTransform.position);
            Vector2 position = screenPoint - screenSize * 0.5f;
            position *= screenAdjustFactor;

            if(preventBlinkError == true)
            {
                CheckError(position);
            }
            else
            {
                rectTransform.anchoredPosition = position;
            }
        }
    }
}
