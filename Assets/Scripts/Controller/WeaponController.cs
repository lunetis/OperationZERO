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
    TargetObject target;

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
    
    bool isFocusingTarget;

    // UI / Misc
    [Header("UI / Misc.")]
    [SerializeField]
    MinimapController minimapController;

    [SerializeField]
    GunCrosshair gunCrosshair;

    [Header("Sounds")]
    [SerializeField]
    AudioClip ammunitionZeroClip;
    [SerializeField]
    AudioClip cooldownClip;

    [SerializeField]
    AudioSource voiceAudioSource;
    [SerializeField]
    AudioSource weaponAudioSource;
    [SerializeField]
    AudioSource missileAudioSource;

    [SerializeField]
    GunAudio gunAudio;

    AircraftController aircraftController;
    UIController uiController;

    public Transform GunTransform
    {
        get { return gunTransform; }
    }

    // For vibration
    Gamepad gamepad;

    // Weapon Callbacks
    public void OnFire(InputAction.CallbackContext context)
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

    public void OnGunFire(InputAction.CallbackContext context)
    {
        switch(context.action.phase)
        {
            case InputActionPhase.Started:
                InvokeRepeating("FireMachineGun", 0, fireInterval);
                gunAudio.IsFiring = true;
                break;

            case InputActionPhase.Canceled:
                CancelInvoke("FireMachineGun");
                gunAudio.IsFiring = false;
                Vibrate(0);
                break;
        }
    }

    public void OnChangeTarget(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Started)
        {
            isFocusingTarget = false;
        }

        // Hold Interaction Performed (0.3s)
        else if(context.action.phase == InputActionPhase.Performed)
        {
            if(target == null) return;

            GameManager.CameraController.LockOnTarget(target.transform);
            isFocusingTarget = true;
        }
        
        else if(context.action.phase == InputActionPhase.Canceled)
        {
            // Hold : Focus
            if(isFocusingTarget == true)
            {
                GameManager.CameraController.LockOnTarget(null);
            }
            // Press : Change Target
            else
            {
                ChangeTarget();
            }
        }
    }

    public void ChangeTarget()
    {
        TargetObject newTarget = GetNextTarget();
        if(newTarget == null)   // No target
        {
            GameManager.CameraController.LockOnTarget(null);
            GameManager.TargetController.ChangeTarget(null);
            gunCrosshair.SetTarget(null);
            target = null;
            
            return;
        }

        if(newTarget != null && newTarget == target) return;

        // Previous Target
        if(target != null)
        {
            target.SetMinimapSpriteBlink(false);
        }

        // Current Target
        target = GetNextTarget();

        target.isNextTarget = false;
        target.SetMinimapSpriteBlink(true);
        GameManager.TargetController.ChangeTarget(target);
        gunCrosshair.SetTarget(target.transform);
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
                        else    // something that is not last and before last index
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

        TargetObject reservedHitTargetObject = gunCrosshair.CheckGunHit();

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.Fire(aircraftController.Speed, gameObject.layer, reservedHitTargetObject);
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
            if(voiceAudioSource.isPlaying == false)
            {
                voiceAudioSource.PlayOneShot(ammunitionZeroClip);
            }
            return;
        }
        // Not available : Beep sound
        if(availableWeaponSlot == null)
        {
            weaponAudioSource.PlayOneShot(cooldownClip);
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
        TargetObject targetObject = (target != null && GameManager.TargetController.IsLocked == true) ? target : null;
        missileScript.Launch(targetObject, aircraftController.Speed + 15, gameObject.layer);
        
        weaponCnt--;

        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        
        missileAudioSource.PlayOneShot(SoundManager.Instance.GetMissileLaunchClip());
    }


    public void OnSwitchWeapon(InputAction.CallbackContext context)
    {
        if(context.action.phase == InputActionPhase.Performed)
        {
            useSpecialWeapon = !useSpecialWeapon;
            SetUIAndTarget();
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

    void SetUIAndTarget(bool playAudio = true)
    {
        Missile switchedMissile = (useSpecialWeapon == true) ? specialWeapon : missile;
        WeaponSlot[] weaponSlots = (useSpecialWeapon == true) ? spwSlots : mslSlots;

        uiController.SetGunText(bulletCnt);
        uiController.SetMissileText(missileCnt);
        uiController.SetSpecialWeaponText(specialWeaponName, specialWeaponCnt);
        uiController.SwitchWeapon(weaponSlots, useSpecialWeapon, switchedMissile, playAudio);
        GameManager.TargetController.SwitchWeapon(switchedMissile);
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
        gamepad = Gamepad.current;
    }

    // Start is called before the first frame update
    void Start()
    {
        uiController = GameManager.UIController;
        
        missilePool = GameManager.Instance.missileObjectPool;
        specialWeaponPool = GameManager.Instance.specialWeaponObjectPool;
        bulletPool = GameManager.Instance.bulletObjectPool;

        missilePool.poolObject = missile.gameObject;
        specialWeaponPool.poolObject = specialWeapon.gameObject;

        useSpecialWeapon = false;

        SetArmament();
        SetUIAndTarget(false);
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
