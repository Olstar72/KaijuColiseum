﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : BaseState
{
    PlayerStateController stateController;


    public RunState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override Type Tick()
    {
        Debug.Log("Run State");

        if (stateController.verticalInput == 0 && stateController.horizontalInput == 0)
        {
            return typeof(IdleState);
        }

        if(stateController.quickAttackInput || stateController.heavyAtatckInput)
        {

            return typeof(AttackState);
        }

        if(stateController.shortDodgeInput || stateController.longDodgeInput)
        {

            return typeof(DodgeState);
        }




        stateController._rb.velocity = stateController.movementDir * (stateController._movementComponent.moveSpeed * stateController.moveAmount);

        return null;
    }
}
