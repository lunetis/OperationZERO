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
    protected float maxhp;
    public bool isNextTarget;

    int lastHitLayer;

    protected List<Missile> lockedMissiles = new List<Missile>();
    protected bool isWarning;
    
    protected MinimapSprite minimapSprite;
    protected Collider objectCollider;
    protected bool isDestroyed;

    protected TargetUI targetUI;

    public TargetUI TargetUI
    {
        get { return targetUI; }
        set { targetUI = value; }
    }

    public ObjectInfo Info
    {
        get { return objectInfo; }
    }

    public List<Missile> LockedMissiles
    {
        get { return lockedMissiles; }
    }

    bool isLocking;
    public bool IsLocking
    {
        get { return isLocking; }
        set { isLocking = value; }
    }

    bool isOnMissleAlert;
    public bool IsOnMissleAlert
    {
        get { return isOnMissleAlert; }
        set { isOnMissleAlert = value; }
    }

    // Public Functions
    public virtual void OnDamage(float damage, int layer, string tag = "")
    {
        hp -= damage;
        lastHitLayer = layer;

        if(lastHitLayer == LayerMask.NameToLayer("Player")) // Hit by Player
        {
            GameManager.UIController.SetLabel(AlertUIController.LabelEnum.Hit);
        }

        if(hp <= 0 && isDestroyed == false)
        {
            hp = 0;
            DestroyObject();
        }
    }

    public virtual void OnMissileAlert()
    {

    }
    
    public virtual void AddLockedMissile(Missile missile)
    {
        lockedMissiles.Add(missile);
    }

    public virtual void RemoveLockedMissile(Missile missile)
    {
        lockedMissiles.Remove(missile);
    }

    // Protected / Private Functions
    protected void CheckMissileDistance()
    {
        bool existWarningMissile = false;
        bool executeWarning = false;
        foreach(Missile missile in lockedMissiles)
        {
            float distance = Vector3.Distance(missile.transform.position, transform.position);

            if(distance < Info.WarningDistance)
            {
                existWarningMissile = true;
                
                if(missile.HasWarned == false)
                {
                    executeWarning = true;
                    missile.HasWarned = true;
                    break;
                }
            }
        }

        if(executeWarning)
        {
            OnMissileAlert();
        }

        if(existWarningMissile == true)
        {
            isWarning = true;
        }
        else
        {
            isWarning = false;
        }
    }

    public void DeleteMinimapSprite()
    {
        if(minimapSprite != null)   
        {
            Destroy(minimapSprite.gameObject);
        }
    }

    public void SetMinimapSpriteVisible(bool visible)
    {
        if(minimapSprite != null)   
        {
            minimapSprite.gameObject.SetActive(visible);
            SetMinimapSpriteBlink(false);
        }
    }

    public void SetMinimapSpriteBlink(bool blink)
    {
        if(minimapSprite != null)   
        {
            minimapSprite.SetMinimapSpriteBlink(blink);
        }
    }

    protected void CommonDestroyFunction()
    {
        isDestroyed = true;

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

    protected virtual void DestroyObject()
    {
        CommonDestroyFunction();
        objectCollider.enabled = false;
        
        Destroy(gameObject);
    }

    protected virtual void AdjustValuesByDifficulty()
    {
        
    }
    
    protected virtual void Start()
    {
        objectCollider = GetComponent<Collider>();

        isEnemy = gameObject.layer != LayerMask.NameToLayer("Player");
        if(isEnemy == true)
        {
            GameManager.TargetController?.CreateTargetUI(this);
            GameManager.Instance?.AddEnemy(this);
        }

        maxhp = hp = objectInfo.HP;
        lastHitLayer = 0;

        for(int i = 0; i < transform.childCount; i++)
        {
            GameObject childObject = transform.GetChild(i).gameObject;
            if(childObject.layer == LayerMask.NameToLayer("Minimap"))
            {
                minimapSprite = childObject.GetComponent<MinimapSprite>();
                break;
            }
        }

        AdjustValuesByDifficulty();
    }

    protected void OnDestroy()
    {
        
    }
}
