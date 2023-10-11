using System.Collections;
using UnityEngine;
using Mirror;
 
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
            GameEventManager.OnGetStats += GetStatsHandler;
        }

        private void Start()
        {
            db = new DBManager();
            StartCoroutine(test());
        }

        IEnumerator test()
        {
            yield return new WaitForSeconds(2f);
            db.GetUserStats();
        }

        private void Update()
        {
            CmdDoAttack();
        }

        private void GetStatsHandler(UserData userData)
        {
            if (userData.error.isError)
            {
                Debug.Log(userData.error.errorText);
                return;
            }

            Debug.Log(userData.playerData.health);
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
