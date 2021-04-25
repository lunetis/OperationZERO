using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    [SerializeField]
    ObjectInfo objectInfo;

    public ObjectInfo Info
    {
        get
        {
            return objectInfo;
        }
    }
    
    private void Start()
    {
        if(gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            GameManager.TargetController.CreateTargetUI(this);
        }
    }

    void OnDestroy()
    {
        // if(GameManager.TargetController != null)
        // {
        //     GameManager.TargetController.RemoveTargetUI(this);
        // }
    }
}
