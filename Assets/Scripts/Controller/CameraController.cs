using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    public enum CameraIndex
    {
        ThirdView,
        FirstView,
        FirstViewWithCockpit,
    }
    
    Vector2 lookInputValue;
    Vector2 lookValue;
    Vector3 thirdPivotOriginPosition;

    [Header("Camera Objects")]
    public Camera[] cameras = new Camera[3];
    public Transform firstViewCameraPivot;
    public Transform thirdViewCameraPivot;

    Camera currentCamera;

    int cameraViewIndex = 0;

    [Header("Camera Lerp")]
    public float lerpAmount;

    float zoomValue;
    public float zoomLerpAmount;
    public float zoomAmount;

    float rollValue;
    public float rollLerpAmount;
    public float rollAmount;
    
    float pitchValue;
    public float pitchLerpAmount;
    public float pitchAmount;

    // UI
    UIController uiController;
    public Transform targetArrowTransform;


    public void Look(InputAction.CallbackContext context)
    {
        lookInputValue = context.ReadValue<Vector2>();
    }

    public void ChangeCameraView(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            lookValue = Vector3.zero;
            cameraViewIndex = (++cameraViewIndex) % 3;
            SetCamera();
        }
    }

    void SetCamera()
    {
        for(int i = 0; i < cameras.Length; i++)
        {
            if(i == cameraViewIndex)
            {
                currentCamera = cameras[i];
            }
            cameras[i].gameObject.SetActive(i == cameraViewIndex);
        }
        
        targetArrowTransform.SetParent(currentCamera.transform, false);
        uiController.SwitchUI((CameraIndex)cameraViewIndex);
    }


    void Rotate1stViewCamera()
    {
        Vector3 rotateValue = new Vector3(lookValue.y * -90, lookValue.x * 180, 0);
        firstViewCameraPivot.localEulerAngles = rotateValue;
        uiController.AdjustFirstViewUI(rotateValue);
    }

    void Rotate1stViewWithCockpitCamera()
    {
        Vector3 rotateValue = new Vector3(lookValue.y * -90, lookValue.x * 135, 0);
        if(rotateValue.x > 0)
            rotateValue.x *= 0.3f;

        firstViewCameraPivot.localEulerAngles = rotateValue;
        uiController.AdjustFirstViewUI(rotateValue);
    }

    void Rotate3rdViewCamera()
    {
        Transform cameraTransform = currentCamera.transform;

        Vector3 rotateValue = new Vector3(lookValue.y * -90, lookValue.x * 180, rollValue * rollAmount);
        Vector3 adjustPosition = new Vector3(0, pitchValue * pitchAmount - Mathf.Abs(lookValue.y) * 1.5f, -zoomValue * zoomAmount);
        
        thirdViewCameraPivot.localEulerAngles = rotateValue;
        thirdViewCameraPivot.localPosition = thirdPivotOriginPosition + adjustPosition;
    }

    public void AdjustCameraValue(float aircraftAccelValue, float aircraftRollValue, float aircraftPitchValue)
    {
        zoomValue = Mathf.Lerp(zoomValue, aircraftAccelValue, zoomLerpAmount * Time.deltaTime);
        rollValue = Mathf.Lerp(rollValue, aircraftRollValue, rollLerpAmount * Time.deltaTime);
        pitchValue = Mathf.Lerp(pitchValue, aircraftPitchValue, pitchLerpAmount * Time.deltaTime);
    }
    
    public Camera GetActiveCamera()
    {
        return currentCamera;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        thirdPivotOriginPosition = thirdViewCameraPivot.localPosition;
        uiController = GameManager.Instance.uiController;
        SetCamera();
    }

    // Update is called once per frame
    void Update()
    {
        lookValue = Vector2.Lerp(lookValue, lookInputValue, lerpAmount * Time.deltaTime);

        switch((CameraIndex)cameraViewIndex)
        {
            case CameraIndex.FirstView:
                Rotate1stViewCamera();
                break;
                
            case CameraIndex.FirstViewWithCockpit:
                Rotate1stViewWithCockpitCamera();
                break;
                
            case CameraIndex.ThirdView:
                Rotate3rdViewCamera();
                break;
        }
    }
}
