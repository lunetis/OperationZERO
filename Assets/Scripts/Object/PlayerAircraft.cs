using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAircraft : TargetObject
{
    public enum WarningStatus
    {
        NONE,
        WARNING,
        MISSILE_ALERT,
        MISSILE_ALERT_EMERGENCY
    }

    [SerializeField]
    float missileEmergencyDistance;

    [SerializeField]
    MissileIndicatorController missileIndicatorController;

    [SerializeField]
    float destroyDelay = 1;

    [SerializeField]
    Transform smokeTransformParent;
    
    [SerializeField]
    GameObject aircraftModel;

    int score = 0;
    UIController uiController;

    public float MissileEmergencyDistance
    {
        get { return missileEmergencyDistance; }
    }

    public override void OnDamage(float damage, int layer)
    {
        base.OnDamage(damage, layer);
        uiController.SetDamage((int)(Info.HP - hp / Info.HP * 100));

        for(int i = 0; i < smokeTransformParent.childCount; i++)
        {
            GameManager.Instance.CreateDamageSmokeEffect(smokeTransformParent.GetChild(i));
        }
    }

    public void OnScore(int score)
    {
        this.score += score;
        uiController.SetScoreText(this.score);
    }
    
    protected override void DestroyObject() 
    {
        CommonDestroyFunction();
        GameManager.Instance.GameOver(false);
        Invoke("DelayedDestroy", destroyDelay);
    }

    public override void AddLockedMissile(Missile missile)
    {
        base.AddLockedMissile(missile);
        missileIndicatorController.AddMissileIndicator(missile);
    }

    void DelayedDestroy()
    {
        GameObject obj = Instantiate(destroyEffect, transform.position, Quaternion.identity);
        // obj.transform.localScale *= 2;
        Destroy(aircraftModel);
        GetComponent<Collider>().enabled = false;
        GetComponent<AircraftController>().enabled = false;
    }

    public void SelfDestruct()
    {
        DestroyObject();
    }


    public WarningStatus GetWarningStatus()
    {
        if(lockedMissiles.Count > 0)
        {
            foreach(Missile missile in lockedMissiles)
            {
                float distance = Vector3.Distance(transform.position, missile.transform.position);
                if(distance < missileEmergencyDistance)
                {
                    return WarningStatus.MISSILE_ALERT_EMERGENCY;
                }
            }
        
            return WarningStatus.MISSILE_ALERT;
        }

        if(IsLocking == true)
        {
            return WarningStatus.WARNING;
        }

        return WarningStatus.NONE;
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        uiController = GameManager.UIController;
        
        uiController.SetDamage(0);
        uiController.SetScoreText(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(isDestroyed == true)
        {
            transform.Rotate(0, 0, 600 * Time.deltaTime);
        }
    }
}
