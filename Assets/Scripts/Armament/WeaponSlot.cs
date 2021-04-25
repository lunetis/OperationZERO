using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSlot
{
    float cooldownTime;
    float currentCooldown;
    float cooldownReciprocal;

    float lastStartCooldownTime;

    public float LastStartCooldownTime
    {
        get
        {
            return lastStartCooldownTime;
        }
    }

    public WeaponSlot(float cooldownTime)
    {
        this.cooldownTime = cooldownTime;
        currentCooldown = cooldownTime;
        cooldownReciprocal = 1 / cooldownTime;

        lastStartCooldownTime = 0;
    }

    // UI purpose
    public float GetCurrentCooldownPercent()
    {
        return currentCooldown * cooldownReciprocal;
    }
    
    // Call when using weapon
    public void StartCooldown()
    {
        currentCooldown = 0;
        lastStartCooldownTime = Time.time;
    }

    // increase 0 to cooldownTime
    public void UpdateCooldown()
    {
        if(currentCooldown < cooldownTime)
        {
            currentCooldown += Time.deltaTime;
            if(currentCooldown > cooldownTime)
                currentCooldown = cooldownTime;
        }
    }

    public bool IsAvailable()
    {
        return currentCooldown >= cooldownTime;
    }
}
