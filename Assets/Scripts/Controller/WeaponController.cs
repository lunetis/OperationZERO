using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(AircraftController))]

public class WeaponController : MonoBehaviour
{
    // Weapon Inputs
    bool useSpecialWeapon;

    // Weapon System
    [Header("Common Weapon System")]
    public TargetObject target;

    [SerializeField]
    float targetDetectDistance;

    [SerializeField]
    Transform leftMissileTransform;
    [SerializeField]
    Transform rightMissileTransform;

    // Missile
    [Header("Missile")]
    [SerializeField]
    Missile missile;
    WeaponSlot[] mslSlots = new WeaponSlot[2];

    ObjectPool missilePool;
    int missileCnt;
    float missileCooldownTime;

    // Special Weapon;
    [Header("Special Weapon")]
    [SerializeField]
    Missile specialWeapon;
    WeaponSlot[] spwSlots = new WeaponSlot[2];

    ObjectPool specialWeaponPool;
    string specialWeaponName;
    int specialWeaponCnt;
    float spwCooldownTime;
    
    // Machine Gun
    [Header("Machine Gun")]
    [SerializeField]
    int bulletCnt;
    [SerializeField]
    Transform gunTransform;
    [SerializeField]
    float gunRPM;
    [SerializeField]
    float vibrateAmount;

    ObjectPool bulletPool;
    float fireInterval;

    // UI / Misc
    [Header("UI / Misc.")]
    [SerializeField]
    MinimapController minimapController;

    AircraftController aircraftController;
    UIController uiController;

    // For vibration
    Gamepad gamepad;

    // Weapon Callbacks
    public void Fire(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            if(useSpecialWeapon == true)
            {
                LaunchMissile(ref specialWeaponCnt, ref specialWeaponPool, ref spwSlots);
            }
            else
            {
                LaunchMissile(ref missileCnt, ref missilePool, ref mslSlots);
            }
        }
    }

    public void GunFire(InputAction.CallbackContext context)
    {
        switch(context.action.phase)
        {
            case InputActionPhase.Performed:
                InvokeRepeating("FireMachineGun", 0, fireInterval);
                break;

            case InputActionPhase.Canceled:
                CancelInvoke("FireMachineGun");
                Vibrate(0);
                break;
        }
    }

    public void ChangeTarget(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            TargetObject newTarget = GetNextTarget();
            if(newTarget == null || (newTarget != null && newTarget == target)) return;

            target = GetNextTarget();
            target.isNextTarget = false;
            GameManager.TargetController.ChangeTarget(target);
        }
    }

    TargetObject GetNextTarget()
    {
        List<TargetObject> targets = GameManager.Instance.GetTargetsWithinDistance(3000);
        TargetObject selectedTarget = null;

        if(targets.Count == 0) return null;

        else if(targets.Count == 1) selectedTarget = targets[0];

        else
        {
            if(target == null) return targets[0];   // not selected

            for(int i = 0; i < targets.Count; i++)
            {
                if(targets[i] == target)
                {
                    if(i == targets.Count - 1)  // last index
                    {
                        targets[1].isNextTarget = true;
                        targets[0].isNextTarget = false;
                        selectedTarget = targets[0];
                    }
                    else
                    {
                        if(i + 1 == targets.Count - 1)  // i + 1 == last index
                        {
                            targets[0].isNextTarget = true;
                        }
                        else
                        {
                            targets[i + 2].isNextTarget = true;
                        }
                        selectedTarget = targets[i + 1];
                    }
                }
            }
        }
        return selectedTarget;
    }


    void FireMachineGun()
    {
        if(bulletCnt <= 0)
        {
            // Beep sound
            CancelInvoke("FireMachineGun");
            return;
        }

        GameObject bullet = bulletPool.GetPooledObject();
        bullet.transform.position = gunTransform.position;
        bullet.transform.rotation = transform.rotation;
        bullet.SetActive(true);

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Fire(aircraftController.Speed, gameObject.layer);
        bulletCnt--;
        
        // Vibration
        Vibrate(vibrateAmount);
        uiController.SetGunText(bulletCnt);
    }

    // Return the oldest-launched WeaponSlot
    // If none of them are available, return null
    WeaponSlot GetAvailableWeaponSlot(ref WeaponSlot[] weaponSlots)
    {
        WeaponSlot oldestSlot = null;

        foreach(WeaponSlot slot in weaponSlots)
        {
            if(slot.IsAvailable() == true)
            {
                if(oldestSlot == null)
                {
                    oldestSlot = slot;
                }
                else if(oldestSlot.LastStartCooldownTime > slot.LastStartCooldownTime)
                {
                    oldestSlot = slot;
                }
            }
        }

        return oldestSlot;
    }


    void LaunchMissile(ref int weaponCnt, ref ObjectPool objectPool, ref WeaponSlot[] weaponSlots)
    {
        WeaponSlot availableWeaponSlot = GetAvailableWeaponSlot(ref weaponSlots);
        
        // Ammunition Zero!
        if(weaponCnt <= 0)
        {
            Debug.Log("Ammunition Zero!");
            return;
        }
        // Not available : Beep sound
        if(availableWeaponSlot == null)
        {
            Debug.Log("Cooldown!");
            return;
        }

        Vector3 missilePosition;
        
        // Select Launch Position
        if(weaponCnt % 2 == 1)
        {
            missilePosition = rightMissileTransform.position;
        }
        else
        {
            missilePosition = leftMissileTransform.position;
        }
        
        // Start Cooldown
        availableWeaponSlot.StartCooldown();

        // Get from Object Pool and Launch
        GameObject missile = objectPool.GetPooledObject();
        missile.transform.position = missilePosition;
        missile.transform.rotation = transform.rotation;
        missile.SetActive(true);

        Missile missileScript = missile.GetComponent<Missile>();
        Transform targetTrasnform = (target != null) ? target.transform : null;
        missileScript.Launch(targetTrasnform, aircraftController.Speed + 15, gameObject.layer);
        
        weaponCnt--;

        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
    }


    public void SwitchWeapon(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            useSpecialWeapon = !useSpecialWeapon;
            SetUI();
        }
    }

    void Vibrate(float vibrateAmount)
    {
        // Vibration
        if(gamepad != null)
        {
            gamepad.SetMotorSpeeds(vibrateAmount, vibrateAmount);
        }
    }

    void SetUI()
    {
        Missile switchedMissile = (useSpecialWeapon == true) ? specialWeapon : missile;
        WeaponSlot[] weaponSlots = (useSpecialWeapon == true) ? spwSlots : mslSlots;

        uiController.SetGunText(bulletCnt);
        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        uiController.SwitchWeapon(weaponSlots, useSpecialWeapon, switchedMissile);
    }

    void SetArmament()
    {
        // Guns
        fireInterval = 60.0f / gunRPM;

        // Missiles
        missileCnt = missile.payload;
        missileCooldownTime = missile.cooldown;
        for(int i = 0; i < 2; i++)
        {
            mslSlots[i] = new WeaponSlot(missileCooldownTime);
        }

        // Special Weapons
        specialWeaponCnt = specialWeapon.payload;
        spwCooldownTime = specialWeapon.cooldown;
        specialWeaponName = specialWeapon.missileName;

        for(int i = 0; i < 2; i++)
        {
            spwSlots[i] = new WeaponSlot(spwCooldownTime);
        }
    }

    void SetMinimapCamera()
    {
        // Minimap
        Vector2 distance = new Vector3(transform.position.x - target.transform.position.x, 
                                       transform.position.z - target.transform.position.z);
        minimapController.SetZoom(distance.magnitude);
    }

    void Awake()
    {
        aircraftController = GetComponent<AircraftController>();
        uiController = GameManager.Instance.uiController;
        gamepad = Gamepad.current;

        missilePool = GameManager.Instance.missileObjectPool;
        specialWeaponPool = GameManager.Instance.specialWeaponObjectPool;
        bulletPool = GameManager.Instance.bulletObjectPool;

        missilePool.poolObject = missile.gameObject;
        specialWeaponPool.poolObject = specialWeapon.gameObject;
    }

    // Start is called before the first frame update
    void Start()
    {
        useSpecialWeapon = false;

        SetArmament();
        SetUI();
    }

    void Update()
    {
        // UI
        foreach(WeaponSlot slot in mslSlots)
        {
            slot.UpdateCooldown();
        }
        foreach(WeaponSlot slot in spwSlots)
        {
            slot.UpdateCooldown();
        }

        if(target != null)
        {
            SetMinimapCamera();
        }
    }
}
