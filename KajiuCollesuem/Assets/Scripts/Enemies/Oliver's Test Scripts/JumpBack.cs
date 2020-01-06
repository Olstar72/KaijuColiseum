﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class JumpBack : MonoBehaviour, IState
{
    private Animator _anim;
    private NavMeshAgent _agent;
    private Transform _target;

    [SerializeField] private float _cooldown = 1.0f;
    private float _nextEnterTime = 0.0f;

    [SerializeField] private float _jumpBackRange = 1;

    public bool _enabled = false;

    public void Setup(Transform pTarget, Animator pAnim, NavMeshAgent pAgent)
    {
        _anim = pAnim;
        _agent = pAgent;
        _target = pTarget;
    }

    public void Enter()
    {
        //Debug.Log("Jump back: Enter");
        _enabled = true;
        //_agent.isStopped = true;
        transform.LookAt(_target.position);
        _agent.Stop();
    }

    public void Exit()
    {
        //Debug.Log("Jump back: Exit");
        _enabled = false;
    }

    public bool CanEnter(float pDistance)
    {
        //If target is a gonner don't enter
        if (_target == null || _target.gameObject.activeSelf == false) return false;

        //Can shoot if cooldown is up and player is in range
        if (Time.time >= _nextEnterTime && pDistance < _jumpBackRange)
            return true;

        return false;
    }

    public bool CanExit(float pDistance)
    {
        //Debug.Log("Jump back: CanExit - " + (_enabled == false));
        return pDistance > _jumpBackRange;
    }

    public void Tick()
    {
        if (_enabled)
        {
            transform.Translate(Vector3.back * 2);
            transform.Translate(Vector3.up);
        }
    }
}
