﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    PlayerStateController stateController;
    
    [SerializeField] private int numberOfClicks = 0;
    [SerializeField] private float lastClickedTime = 0;
    [SerializeField] private float maxComboDelay = 0.9f;

    private float AttackStateReturnDelayLength = 0.6f;

    public AttackState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {
        Debug.Log("AttackState: Enter");
        //stateController._hitboxComponent.gameObject.SetActive(true); /* Handled by animation events */

        CheckForAttack();
    }

    public override void Exit()
    {
        Debug.Log("AttackState: Exit");
        //stateController._hitboxComponent.gameObject.SetActive(false); /* Handled by animation events */
        stateController.AttackStateReturnDelay = Time.time + AttackStateReturnDelayLength;
        stateController._hitboxComponent.gameObject.SetActive(false);
        numberOfClicks = 0;
        stateController._animHandler.ClearAttackBools();
    }

    public override Type Tick()
    {
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            return typeof(MovementState);
        }

        CheckForAttack();

        // State Switched with Animation Events
        //switch (stateController._animHandler.attackState)
        //{
        //    case 0:
        //        ClearInputs();
        //        break;

        //    case 1:
        //        Attack();
        //        break;

        //    case 2:
        //        done = true;
        //        break;
        //}

        //Stunned
        if (stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        //Respawn
        if (stateController.Respawn)
        {
            stateController.Respawn = false;
            stateController._animHandler.Respawn();
            return typeof(MovementState);
        }

        return null;
    }

    private void CheckForAttack()
    {
        if (numberOfClicks <= 2)
        {
            if (stateController.lightAttackinput == 1)
            {
                lastClickedTime = Time.time;
                numberOfClicks++;

                ClearInputs();
                stateController._animHandler.ClearAttackBools();
                string boolName = "Square" + (numberOfClicks).ToString();
                stateController._animHandler.StartAttack(boolName);
            }
            else if (stateController.heavyAttackinput == 1)
            {
                lastClickedTime = Time.time;
                numberOfClicks++;

                ClearInputs();
                stateController._animHandler.ClearAttackBools();
                string boolName = "Triangle" + (numberOfClicks).ToString();
                stateController._animHandler.StartAttack(boolName);
            }
        }

        //if (stateController.powerInput > 0)
        //{
        //    // run code from the power component
        //    int whichPower = stateController._powerComponent.UsingPower(stateController.powerInput);

        //    if (whichPower == -1)
        //    {
        //        done = true;
        //        Debug.Log("No In Slot Power");
        //    }
        //    else if (whichPower == -2)
        //    {
        //        done = true;
        //        Debug.Log("Not Enough Power");
        //    }
        //    else
        //    {
        //        stateController._animHandler.StartPower(whichPower);
        //        combo = 3;
        //    }
        //}
    }

    private void ClearInputs()
    {
        stateController.lightAttackinput = -1.0f;
        stateController.heavyAttackinput = -1.0f;
    }
}