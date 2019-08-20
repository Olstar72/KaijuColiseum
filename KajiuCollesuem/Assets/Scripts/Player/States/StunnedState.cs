﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : BaseState
{
    PlayerStateController stateController;

     private float timer = 0;

    public StunnedState(PlayerStateController controller) : base(controller.gameObject)
    {
        stateController = controller;
    }

    public override void Enter()
    {

    }

    public override void Exit()
    {

    }

    public override Type Tick()
    {
        Debug.Log("Stunned State");

        timer += Time.deltaTime;
        Debug.Log(10 - timer);

        if (timer >= 10)
        {
            timer = 0;
            return typeof(MovementState);
        }

        return null;
    }
}