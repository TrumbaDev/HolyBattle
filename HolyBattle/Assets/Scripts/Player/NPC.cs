//using OpenCover.Framework.Model;
using palladinTank;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public int num, PlayerID, Lvl, Rare, Position;
    public string NameNPC;

    protected Dictionary<Type, INPCBehaviour> _behaviourMap;
    protected INPCBehaviour _behaviourCurrent;
    private Animator _animator;
    protected float _power, _dexterity, _intelligence, _health, _base_damage, _attack_speed, _armor, _magic_resistance, _move_speed;//_health, _armor, _mageResist, _strenght, _intellect, _agility, _moveSpeed, _attackSpeed, _baseDamage;
    //временная переменная для тестов
    protected float _enemyHealth;


    DBManager db;

    void Start()
    {
        //Создаем экземпляр класса чтобы кинуть запрос
        db = new DBManager();
        //Подписываемся на event чтобы узнать ответ
        GameEventManager.OnGetStats += GetStatsHandler;
        InitBehaviors();
        _animator = GetComponent<Animator>();
        //Корутина не обязательна, нужна просто для тестов чтобы выждать 2 секунды
        //На самом деле можно кидать запрос откуда угодно
        //StartCoroutine(test());
        
    }

    protected virtual void Update()
    {
        _behaviourCurrent?.Update();
    }

    IEnumerator test()
    {
        yield return new WaitForSeconds(2f);
        db.GetUserStats();
    }

    private void GetStatsHandler(UserData userData)
    {
        //В случае если ошибка, то обрабатываем, иначе обрабатываем нужные значения
        if (userData.error.isError)
        {
            Debug.Log(userData.error.errorText);
            return;
        }

        //Debug.Log(userData.playerData.health);
        Debug.Log(userData.npcData.Rare);
    }

    #region ParametrsWork

    /*public void InitializedParametrNPC(string _nameParametr, float _valueParametr)
    {
        switch (_nameParametr)
        {
            case "_health":
                _health += _valueParametr;
                break;
            case "_armor":
                _armor += _valueParametr;
                break;
            case "_mageResist":
                _mageResist += _valueParametr;
                break;
            case "_strenght":
                _strenght += _valueParametr;
                break;
            case "_intellect":
                _intellect += _valueParametr;
                break;
            case "_agility":
                _agility += _valueParametr;
                break;
            case "_moveSpeed":
                _moveSpeed += _valueParametr;
                break;
            case "_attackSpeed":
                _attackSpeed += _valueParametr;
                break;
            case "_baseDamage":
                _baseDamage += _valueParametr;
                break;
        }
    }*/

    #endregion

    #region AnimationWork

    protected virtual void InitBehaviors()
    {
        _behaviourMap = new Dictionary<Type, INPCBehaviour>();

        _behaviourMap[typeof(AttackBehaviour)] = new AttackBehaviour(this);

    }

    public void GetOutOfState()
    {
        _behaviourCurrent = null;
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
        if (_behaviourCurrent == GetBehaviour<AttackBehaviour>())
        {
            _behaviourCurrent.Exit();
        }
        //SetBehaviour(_behaviourMap[null]);
    }

    protected INPCBehaviour GetBehaviour<T>() where T : INPCBehaviour
    {
        var type = typeof(T);
        return _behaviourMap[type];
    }

    protected void SetBehaviour(INPCBehaviour newBehaviour)//, Class _newBehaviour)
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

    #endregion
}
