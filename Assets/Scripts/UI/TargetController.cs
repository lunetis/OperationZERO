using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject targetUIObject;
    List<TargetUI> targetUIs = new List<TargetUI>();
    TargetUI currentTargettedUI;

    [SerializeField]
    TargetObject lockedTarget;

    [SerializeField]
    TargetArrow targetArrow;

    void Start()
    {
        if(lockedTarget != null)
        {
            GameManager.UIController.SetTargetText(lockedTarget.Info);  // Set Upper Left UI
            targetArrow.SetTarget(lockedTarget);    // Set Arrow UI
        }
    }

    public void CreateTargetUI(TargetObject targetObject)
    {
        GameObject obj = Instantiate(targetUIObject);
        TargetUI targetUI = obj.GetComponent<TargetUI>();
        targetUI.Target = targetObject;
        targetUIs.Add(targetUI);

        obj.transform.SetParent(transform, false);
    }

    public void RemoveTargetUI(TargetObject targetObject)
    {
        TargetUI targetUI = FindTargetUI(targetObject);
        if(targetUI.Target != null)
        {
            targetUIs.Remove(targetUI);
            Destroy(targetUI.gameObject);
        }
    }

    public void ChangeTarget(TargetObject newTarget)
    {
        lockedTarget = newTarget;
        targetArrow.SetTarget(lockedTarget);
        GameManager.UIController.SetTargetText(lockedTarget.Info);
        
        TargetUI targetUI = FindTargetUI(lockedTarget);

        Debug.Log("Search : " + lockedTarget);

        if(targetUI != null && targetUI.Target != null)
        {
            if(currentTargettedUI != null)
            {
                currentTargettedUI.SetTargetted(false); // Disable Prev Target
            }
            currentTargettedUI = targetUI;
            currentTargettedUI.SetTargetted(true);    // Enable Current Target
        }
    }

    public void ShowTargetArrow(bool show)
    {
        targetArrow.SetArrowVisible(show);
    }

    public TargetUI FindTargetUI(TargetObject targetObject)
    {
        foreach(TargetUI targetUI in targetUIs)
        {
            if(targetUI.Target == lockedTarget)
            {
                return targetUI;
            }
        }

        return null;
    }
}
