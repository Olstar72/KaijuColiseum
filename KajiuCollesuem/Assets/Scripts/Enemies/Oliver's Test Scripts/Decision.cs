﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
 * Programmer: Mugiesshan Anandarajah
 * Description: Edited to complete the following tasks:
 *      Task 1: Grunts targeting is updated to allow for switching between targets
 *      Task 2: Grunts have a harder time detecting a player
 * */
public class Decision : MonoBehaviour
{
    private IState[] _states;
    private IState _currentState;

    public Transform target;
    private GameObject[] _players;

    public float fScanVision = 30;

    void Start()
    {
        //Get States
        _states = GetComponents<IState>();

        //Get players in the level
        _players = GameObject.FindGameObjectsWithTag("Player");

        //Setup States
        SetupStates();

        //Start on least priority State that can be entered
        StartLastState();
    }

    private void StartLastState()
    {
        //Enter least priority State that can be entered
        for (int i = _states.Length - 1; i >= 0; i--)
        {
            //Get distance to target
            float distance = Vector3.Distance(transform.position, target.position);

            //Check if can Enter
            if (_states[i].CanEnter(distance))
            {
                _currentState = _states[i];
                _currentState.Enter();
                break;
            }
        }
    }

    private void FixedUpdate()
    {
        CheckStates();
        CheckAndUpdateTarget();
    }

    private void Update()
    {
        _currentState.Tick();
    }

    public void SetupStates()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Animator anim = GetComponent<Animator>();

        foreach (IState state in _states)
        {
            state.Setup(target, anim, agent);
        }
        //transform.LookAt(target);
    }

    private void CheckStates()
    {
        //Get distance to target
        float distance = Vector3.Distance(transform.position, target.position);

        //Return if you can't Exit current state
        if (_currentState.CanExit(distance) == false) return;

        foreach (IState state in _states)
        {
            //Check if can stay in same state
            if (_currentState == state)
            {
                if (state.CanEnter(distance))
                    break;
                else
                    continue;
            }

            //Check if state can be entered. Task 2: Grunts have a harder time detecting a player
            if (state.CanEnter(distance)
                && Vector3.Angle(transform.forward, target.position - transform.position) < fScanVision)
            {
                SwitchState(state);
                break;
            }

            else
                SwitchState(GetComponent<Guard>());
        }
        Debug.Log(distance);
    }

    /*Calculate the distance between itself and the player, and updates its target to the nearest player,
    and update the target setup
    Task 1: Grunts targeting is updated to allow for switching of targets*/
    private void CheckAndUpdateTarget()
    {
        float smallest_distance = 9999;
        int index = -1;

        for (int i = 0; i < _players.Length; ++i)
        {
            float distance = Vector3.Distance(transform.position, _players[i].transform.position);

            if (distance < smallest_distance)
            {
                smallest_distance = distance;
                index = i;
            }
        }

        if (_currentState.CanEnter(smallest_distance))
        {
            target = _players[index].transform;
            SetupStates();
        }
    }

    //Exit old state and Enter new state
    private void SwitchState(IState pState)
    {
        _currentState.Exit();
        pState.Enter();
        _currentState = pState;
    }

    // Public Functions //////////////////////////////
    //Called when needing to switch to a state that is not a state the AI wants to be in (stunned, hurt, etc.)
    public void ForceStateSwitch(IState pState)
    {
        SwitchState(pState);
    }

    //Resets the AI for respawning
    public void Respawn()
    {
        _currentState.Exit();
        StartLastState();
    }
}
