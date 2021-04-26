using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    [SerializeField]
    ObjectInfo objectInfo;

    bool isEnemy;
    public bool isNextTarget;

    public ObjectInfo Info
    {
        get
        {
            return objectInfo;
        }
    }
    
    private void Start()
    {
        isEnemy = gameObject.layer != LayerMask.NameToLayer("Player");
        if(isEnemy == true)
        {
            GameManager.TargetController.CreateTargetUI(this);
            GameManager.Instance.AddEnemy(this);
        }
    }

    void OnDestroy()
    {
        if(GameManager.TargetController != null)
        {
            GameManager.TargetController.RemoveTargetUI(this);
        }
        if(isEnemy == true)
        {
            GameManager.Instance.RemoveEnemy(this);
        }
    }
}
