﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EZCameraShake;

public class PlayerHitbox : MonoBehaviour
{
    [SerializeField] private int lightAttackDamage_S1 = 1;
    [SerializeField] private int lightAttackDamage_S2 = 1;
    [SerializeField] private int lightAttackDamage_S3 = 1;
    [SerializeField] private int heavyAttackDamage = 2;
    private int damage;

    [SerializeField] private float damageMultiplier = 1;

    [Space]
    [SerializeField] private int powerRecivedOnHit = 20;

    private PlayerAttributes playerAttributes;
    private PlayerLockOnScript lockOnScript;

    private void Start()
    {
        playerAttributes = GetComponentInParent<PlayerAttributes>();
        lockOnScript = playerAttributes.GetComponent<PlayerLockOnScript>();
    }

    private void OnTriggerEnter (Collider other)
    {
        //Check if collided with an Attributes Script
        IAttributes otherAttributes = other.GetComponent<IAttributes>();

        if (otherAttributes != null && otherAttributes.IsDead() == false)
        {
            //Damage other
            if (otherAttributes.TakeDamage(damage, true))
                //If other died and is lockOn target return camera to default
                lockOnScript.TargetDead(other.transform);

            //Recieve Power
            playerAttributes.RecivePower(powerRecivedOnHit);

            //Camera Shake
            CameraShaker.Instance.ShakeOnce(1, 0.5f, 0.2f, 0.1f);
        }

        if (other.gameObject.name.Equals("Fireball"))
        {
            playerAttributes.TakeDamage(damage, true);
        }
    }

    public void SetDamage(int pIndex)
    {
        switch (pIndex)
        {
            case 1:
                damage = lightAttackDamage_S1;
                break;

            case 2:
                damage = lightAttackDamage_S2;
                break;

            case 3:
                damage = lightAttackDamage_S3;
                break;

            case 4:
                damage = heavyAttackDamage;
                break;
        }

        damage = Mathf.RoundToInt(damage * damageMultiplier);
    }

    //For Upgrading Attack Damage
    public void SetDamageMultiplier(float pMult) => damageMultiplier = pMult;
}