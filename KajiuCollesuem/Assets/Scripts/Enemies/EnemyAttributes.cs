﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAttributes : MonoBehaviour, IAttributes
{
    [SerializeField] private int startHealth = 100;
    private int currentHealth;
    
    [SerializeField] private float healthDisplayLength = 2f;

    private Slider healthSlider;
    [SerializeField] private GameObject enemyHealthBar;

    private bool isDead;

    // Use this for initialization
    void Start()
    {
        currentHealth = startHealth;
        
        if (enemyHealthBar)
        {
            healthSlider = enemyHealthBar.GetComponentInChildren<Slider>();
            enemyHealthBar.SetActive(false);
        }
    }

    public bool TakeDamage(int pAmount, bool pReact)
    {
        currentHealth -= pAmount;

        if (healthSlider)
            healthSlider.value = (float)currentHealth/startHealth;

        //Show Health Bar
        StopCoroutine("ShowHealthbar");
        StartCoroutine("ShowHealthbar");

        //Dead
        if (currentHealth <= 0 && !isDead)
        {
            Death();
            return true;
        }

        //Return Alive
        return false;
    }

    IEnumerator ShowHealthbar()
    {
        enemyHealthBar.SetActive(true);
        yield return new WaitForSeconds(healthDisplayLength);
        enemyHealthBar.SetActive(false);
    }

    void Death()
    {
        isDead = true;
        Destroy(gameObject);
    }
}
