//using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    private Dictionary<Type, INPCBehaviour> _behaviourMap;
    private INPCBehaviour _behaviourCurrent;
    private Animator _animator;

    void Start()
    {
        InitBehaviors();
        _animator = GetComponent<Animator>();
    }

    private void InitBehaviors()
    {
        _behaviourMap = new Dictionary<Type, INPCBehaviour>();

        _behaviourMap[typeof(AttackBehaviour)] = new AttackBehaviour(this);

    }

    public void OnTriggerEnterMovement()//bool _isTrigger)
    {
        //if (_isTrigger)
        //{
        SetBehaviour(_behaviourMap[typeof(AttackBehaviour)]);
        //}
    }

    public void OnTriggerExitMovement()
    {
        if (_behaviourCurrent == _behaviourMap[typeof(AttackBehaviour)])
        {
            _behaviourCurrent.Exit();
        }
        //SetBehaviour(_behaviourMap[null]);
    }

    /*private INPCBehaviour GetBehaviour<T>() where T : INPCBehaviour
    {
        var type = typeof(T);
        return _behaviourMap[type];
    }*/

    private void SetBehaviour(INPCBehaviour newBehaviour)//, Class _newBehaviour)
    {
        if (_behaviourCurrent != null)
            _behaviourCurrent.Exit();

        _behaviourCurrent = newBehaviour;
        _behaviourCurrent.Enter();
    }

    public void SetAnimationNPC(string _typeParameter, string _nameAnimation, bool _boolAnimation)
    {
        switch (_typeParameter)
        {
            case "bool":
                _animator.SetBool(_nameAnimation, _boolAnimation);
                break;

            default:
                Debug.Log("Тип параметра не определен, скрипт НПС SetAnimationNPC");
                break;
        }
    }
}
