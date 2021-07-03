using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetController : MonoBehaviour
{
    public GameObject targetUIObject;
    List<TargetUI> targetUIs = new List<TargetUI>();
    TargetUI currentTargettedUI;
    TargetObject lockedTarget;

    [SerializeField]
    TargetArrow targetArrow;

    [SerializeField]
    TargetLock targetLock;

    public bool IsLocked
    {
        get { return targetLock.IsLocked; }
    }

    public void CreateTargetUI(TargetObject targetObject)
    {
        if(targetObject.TargetUI != null) return;
        
        GameObject obj = Instantiate(targetUIObject);
        TargetUI targetUI = obj.GetComponent<TargetUI>();
        targetUI.Target = targetObject;
        targetObject.TargetUI = targetUI;
        targetUIs.Add(targetUI);

        obj.transform.SetParent(transform, false);
    }


    // Remove from TargetUI List, stop functioning and Destroy
    public void RemoveTargetUI(TargetObject targetObject)
    {
        TargetUI foundUI = null;
        foreach(TargetUI targetUI in targetUIs)
        {
            if(targetUI.Target == targetObject)
            {
                foundUI = targetUI;
                break;
            }
        }

        if(foundUI != null)
        {
            targetUIs.Remove(foundUI);
            Destroy(foundUI.gameObject);
        }
    }

    public void RemoveAllTargetUI()
    {
        if(targetUIs.Count == 0) return;
        
        foreach(TargetUI targetUI in targetUIs)
        {
            Destroy(targetUI.gameObject);
        }
        targetUIs.Clear();
    }

    public void ChangeTarget(TargetObject newTarget)
    {
        // No target
        if(newTarget == null)
        {
            targetArrow.SetTarget(null);
            GameManager.UIController.SetTargetText(null);
            targetLock.SetTarget(null);
            return;
        }

        lockedTarget = newTarget;
        targetArrow.SetTarget(lockedTarget);
        GameManager.UIController.SetTargetText(lockedTarget.Info);
        
        TargetUI targetUI = FindTargetUI();
        targetLock.SetTarget(lockedTarget.transform);


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

    public void SwitchWeapon(Missile missile)
    {
        targetLock.SwitchWeapon(missile);
    }

    public void ShowTargetArrow(bool show)
    {
        targetArrow.SetArrowVisible(show);
    }

    public TargetUI FindTargetUI()
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

    public void SetTargetUILock(bool isLocked)
    {
        if(currentTargettedUI)
        {
            currentTargettedUI.SetLock(isLocked);
        }
    }

    void Start()
    {
        if(lockedTarget != null)
        {
            GameManager.UIController.SetTargetText(lockedTarget.Info);  // Set Upper Left UI
            targetArrow.SetTarget(lockedTarget);    // Set Arrow UI
        }
    }
}
