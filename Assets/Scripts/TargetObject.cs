using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetObject : MonoBehaviour
{
    [SerializeField]
    protected ObjectInfo objectInfo;

    [SerializeField]
    protected GameObject destroyEffect;

    protected bool isEnemy;
    protected float hp;
    public bool isNextTarget;

    int lastHitLayer;

    public ObjectInfo Info
    {
        get
        {
            return objectInfo;
        }
    }

    protected void DeleteMinimapSprite()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject childObject = transform.GetChild(i).gameObject;
            if(childObject.layer == LayerMask.NameToLayer("Minimap"))
            {
                Destroy(childObject);
            }
        }
    }

    protected void CommonDestroyFunction()
    {
        GameObject obj = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        if(isEnemy == true)
        {
            GameManager.Instance?.RemoveEnemy(this);
            GameManager.TargetController?.RemoveTargetUI(this);
            GameManager.WeaponController?.ChangeTarget();

            if(lastHitLayer == LayerMask.NameToLayer("Player"))
            {
                GameManager.PlayerAircraft.OnScore(objectInfo.Score);
                GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Destroyed);
            }
        }
        DeleteMinimapSprite();
    }

    public virtual void OnDamage(float damage, int layer)
    {
        hp -= damage;
        lastHitLayer = layer;

        if(lastHitLayer == LayerMask.NameToLayer("Player")) // Hit by Player
        {
            GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Hit);
        }

        if(hp <= 0)
        {
            DestroyObject();
        }
    }

    protected virtual void DestroyObject()
    {
        CommonDestroyFunction();
        Destroy(gameObject);
    }
    
    protected virtual void Start()
    {
        isEnemy = gameObject.layer != LayerMask.NameToLayer("Player");
        if(isEnemy == true)
        {
            GameManager.TargetController.CreateTargetUI(this);
            GameManager.Instance.AddEnemy(this);
        }

        hp = objectInfo.HP;
        lastHitLayer = 0;
    }

    protected void OnDestroy()
    {
        
    }
}
