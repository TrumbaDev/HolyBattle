using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.Networking;
 
namespace GamePlayer
{
    public class Player : NetworkBehaviour
    {
        [SyncVar]
        [SerializeField] private float _attackRange;

        [SyncVar]
        [SerializeField] private GameObject _projectile;


        DBManager db;

        private Animator _playerAnim;

        private void Awake()
        {
            _playerAnim = GetComponent<Animator>();
        }

        private void Start()
        {
            db = new DBManager();
        }

        private void Update()
        {
            CmdDoAttack();
        }

        private void OnCollisionEnter(Collision collision)
        {
            CmdOnCollisionEnter(collision);
        }


        [Command] //Обработка события коллизии на сервере
        private void CmdOnCollisionEnter(Collision collision)
        {

        }

        [Command]
        private void CmdDoAttack()
        {
            if (_playerAnim.GetFloat("Walk") <= _attackRange)
            {
            
            }
        }

    }

}
