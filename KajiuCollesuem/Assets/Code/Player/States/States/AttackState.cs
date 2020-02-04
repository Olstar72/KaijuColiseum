﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    PlayerStateController _stateController;

    private int _numberOfClicks = 0;
    private float _exitStateTime = 0;
    private float _addForceTime = 0;
    private float _stopForceTime = 0;
    private float _addForceAmount = 0;

    private float _enableHitboxTime = 0;
    private float _disableHitboxTime = 0;
    private PlayerHitbox _hitbox = null;

    private float _attackStateReturnDelayLength = 0.2f;
    private float _maxCharge = 1.0f;

    private bool _onHolding = false;
    public float chargeTimer = 0f;

    public AttackState(PlayerStateController controller) : base(controller.gameObject)
    {
        _stateController = controller;
    }

    public override void Enter()
    {
        //Debug.Log("AttackState: Enter");
        //stateController._hitboxComponent.gameObject.SetActive(true); /* Handled by animation events */
        _exitStateTime = 0;
        _onHolding = false;
        CheckForAttack();
    }

    public override void Exit()
    {
        //Debug.Log("AttackState: Exit");
        _stateController.AttackStateReturnDelay = Time.time + _attackStateReturnDelayLength;
        _numberOfClicks = 0;

        // If leaving state before disabling hitbox, disable hitbox
        if (_hitbox != null)
        {
            _hitbox.gameObject.SetActive(false);
            _hitbox = null;
        }
    }

    public override Type Tick()
    {
        // Check for another attack
        CheckForAttack();

        // Leave state after attack
        if (Time.time > _exitStateTime && _onHolding == false)
        {
            return typeof(MovementState);
        }

        // Forward stepping force
        if (Time.time > _addForceTime && Time.time < _stopForceTime && _onHolding == false)
        {
            _stateController._rb.AddForce(_stateController._modelController.transform.forward * _addForceAmount);
        }

        // Hitbox enable & disable
        if (_hitbox != null)
        {
            if (Time.time >= _disableHitboxTime)
            {
                _hitbox.gameObject.SetActive(false);
                _hitbox = null;
            }
            else if (Time.time >= _enableHitboxTime)
            {
                _hitbox.gameObject.SetActive(true);
            }
        }

        // Stunned
        if (_stateController.Stunned)
        {
            return typeof(StunnedState);
        }

        return null;
    }

    public void CheckForAttack()
    {
        // Cannot go beyond combo limit
        if (_numberOfClicks <= 2)
        {
            // If holding don't listen for more attacks, listen for release else run holding code.
            if (_onHolding == true)
            {
                // ON RELEASE HEAVY (Called Once)
                if (_stateController.heavyAttackinput == 0)
                {
                    ReleaseHeavyAttack();
                    ClearInputs();
                }
                // ON HOLDING HEAVY
                else
                {
                    chargeTimer += Time.deltaTime;

                    // If Reached max charge release heavy attack
                    if (chargeTimer >= _maxCharge)
                    {
                        _stateController.ignoreNextHeavyAttackRelease = true;
                        ReleaseHeavyAttack();
                    }
                }
            }
            // Can only input for next attack if done previous attack
            else if (Time.time > _exitStateTime || _exitStateTime == 0)
            {
                // ON RELEASED HEAVY BEFORE CHARGING STARTED (Called Once)
                if (_stateController.heavyAttackinput == 0)
                {
                    PressedHeavyAttack();
                    ReleaseHeavyAttack();
                    ClearInputs();
                }
                // ON PRESSED HEAVY (Called Once)
                else if (_stateController.heavyAttackinput == 1)
                {
                    PressedHeavyAttack();
                    ClearInputs();
                }
                // ON PRESSED LIGHT (Called Once)
                else if (_stateController.lightAttackinput == 1)
                {
                    PressedLightAttack();
                    ClearInputs();
                }
            }
        }
    }

    #region Pressed & Release
    private void PressedLightAttack()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks];
        float PreAttackTime = curAttack.transitionToTime + curAttack.holdStartPosTime;
        SetAttackValues(curAttack, PreAttackTime);

        _stateController._modelController.PlayAttack(_numberOfClicks, false, false);

        _numberOfClicks++;
    }

    private void PressedHeavyAttack()
    {
        _stateController._modelController.PlayAttack(_numberOfClicks, true, true);

        _exitStateTime = 0;
        chargeTimer = 0;
        _onHolding = true;
    }

    private void ReleaseHeavyAttack()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks + 3];
        SetAttackValues(curAttack); // TODO Add charging mult to hitbox

        // TODO send through how long attack was charged for and use that to know how fast the attack should move.
        _stateController._modelController.DoneChargingAttack();

        _onHolding = false;
        _numberOfClicks++;
    }

    private void ReleaseHeavyAttackBeforeCharging()
    {
        SOAttack curAttack = _stateController._modelController.attacks[_numberOfClicks + 3];
        float PreAttackTime = curAttack.transitionToTime + curAttack.holdStartPosTime;
        SetAttackValues(curAttack, PreAttackTime);

        _stateController._modelController.PlayAttack(_numberOfClicks, true, false);

        _numberOfClicks++;
    }
    #endregion

    #region Set & Clear
    private void SetAttackValues(SOAttack pAttackVars, float pPreAttackTime = 0)
    {
        // Timings
        _exitStateTime = Time.time + pAttackVars.attackTime + pPreAttackTime + pAttackVars.holdEndPosTime;
        _addForceTime = Time.time + pAttackVars.forceForwardTime + pPreAttackTime;
        _stopForceTime = Time.time + pAttackVars.stopForceForwardTime + pPreAttackTime;
        _addForceAmount = pAttackVars.forceForwardAmount;

        // Hitbox
        _hitbox = _stateController.hitboxes[pAttackVars.hitboxIndex];
        Vector3 knockVector = _stateController._modelController.transform.forward * pAttackVars.HitboxKnockback + Vector3.up * pAttackVars.HitboxKnockup;
        _hitbox.SetDamage(pAttackVars.HitboxDamage, knockVector);

        _enableHitboxTime = Time.time + pAttackVars.enableHitboxTime + pPreAttackTime;
        _disableHitboxTime = Time.time + pAttackVars.disableHitboxTime + pPreAttackTime;
    }

    private void ClearInputs()
    {
        _stateController.lightAttackinput = -1.0f;
        _stateController.heavyAttackinput = -1.0f;
    }
    #endregion
}