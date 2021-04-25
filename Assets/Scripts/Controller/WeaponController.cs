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
    public Transform target;
    public Transform leftMissileTransform;
    public Transform rightMissileTransform;

    // Missile
    [Header("Missile")]
    public Missile missile;
    WeaponSlot[] mslSlots = new WeaponSlot[2];

    ObjectPool missilePool;
    int missileCnt;
    float missileCooldownTime;

    // Special Weapon;
    [Header("Special Weapon")]
    public Missile specialWeapon;
    WeaponSlot[] spwSlots = new WeaponSlot[2];

    ObjectPool specialWeaponPool;
    string specialWeaponName;
    int specialWeaponCnt;
    float spwCooldownTime;
    
    // Machine Gun
    [Header("Machine Gun")]
    public int bulletCnt;
    public Transform gunTransform;
    public float gunRPM;
    public float vibrateAmount;

    ObjectPool bulletPool;
    float fireInterval;

    // UI / Misc
    [Header("UI / Misc.")]
    public MinimapController minimapController;

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
        missileScript.Launch(target, aircraftController.Speed + 15, gameObject.layer);
        
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
        Vector2 distance = new Vector3(transform.position.x - target.position.x, 
                                       transform.position.z - target.position.z);
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
