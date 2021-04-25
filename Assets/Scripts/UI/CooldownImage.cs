using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownImage : MonoBehaviour
{
    public Image frameImage;
    public Image fillImage;

    float remainCooldown;
    float maxCooldown;
    float cooldownReciprocal;

    WeaponSlot weaponSlot;

    public void SetWeaponData(WeaponSlot weaponSlot, Sprite frameSprite, Sprite fillSprite)
    {
        this.weaponSlot = weaponSlot;
        frameImage.sprite = frameSprite;
        fillImage.sprite = fillSprite;
    }

    public void SetColor(Color color)
    {
        frameImage.color = color;
        fillImage.color = color;
    }

    public void StartCooldown(float cooldown)
    {
        maxCooldown = cooldown;
        remainCooldown = 0;
        cooldownReciprocal = 1 / cooldown;
    }

    // Start is called before the first frame update
    void Start()
    {
        remainCooldown = maxCooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        fillImage.fillAmount = weaponSlot.GetCurrentCooldownPercent();
    }
}
