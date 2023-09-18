using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;
using UnityEngine.Networking;

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
        StartCoroutine(test());
    }

    private void Update()
    {
        CmdDoAttack();
    }

    public IEnumerator test()
    {
        string[] formValue = { "give_me_skills" };
        UnityWebRequest rq = db.ConnectToDB(formValue);
        yield return rq.SendWebRequest();
        Debug.Log(rq.downloadHandler.text);
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
