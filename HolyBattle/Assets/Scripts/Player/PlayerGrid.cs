//Функция работающая с сеткой спавна игрока

//позиция спавна должна быть определена системой на этапе регистрации в бой
//при пве у игрока точка №1, при пвп рандом
//команды поделены следующим спавном: команда 1 - точки спавна 1, 2, 3
//команда 2 - точки спавна 2, 4, 6
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;
using UnityEngine.Networking;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class PlayerGrid : NetworkBehaviour
{
    public string _teamNum { get; private set; }
    public string _xPos { get; private set; }

    [SerializeField] public List<GameObject> _NPCGrid;
    [SerializeField] private List<GameObject> _SpawnSpots;
    [SerializeField] private GameObject _cube;

    private Transform _cameraTransform;
    private List<GameObject> _netRegistredPrefab;
    private DBManager _dbManager;
    
    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _netRegistredPrefab = GameObject.Find("NetMan").GetComponent<NetMan>().spawnPrefabs;
        GameEventManager.OnGetStats += GetStatsHandler;
    }

    private void Start()
    {
        float _zPos = transform.position.z;
        _xPos = transform.position.x.ToString();
        
        _dbManager = new DBManager();
        StartCoroutine(test1());

        if (_zPos <= 40)
        {
            _teamNum = "Team1";
            SetCameraForPlayer(new Vector3(42.5f, 13.72f, 26.25f), new Vector3(33.45f, 0f, 0f));
        }
        else
        {
            _teamNum = "Team2";
            SetCameraForPlayer(new Vector3(42.5f, 13.72f, 58.86f), new Vector3(33.45f, 180f, 0f));
        }

        SetNPCPosition();

        GameObjectsManager.RegisterGrid(gameObject);
        GameEventManager.GridDestroy += NPCGridRemove;

        Invoke("ColliderDisable", 2f);
    }

    public IEnumerator test1()
    {
       /* string[] formValue = { "give_me_NPC_setup" };
        UnityWebRequest rq = _dbManager.ConnectToDB(formValue);
        yield return rq.SendWebRequest();
        Debug.Log(rq.downloadHandler.text);*/
        //Debug.Log(JsonUtility.FromJson<NPCInfoPhP>(rq.downloadHandler.text));
        //"Array\n(\n    [0] => Array\n        (\n            [PlayerID] => 1\n            [Position1] => Paladin/Paladin\n
        //[Position2] => \n            [Position3] => Mage/Mage\n            [Position4] => \n            [Position5] => Paladin/Paladin\n
        //[Position6] => Mage/Mage\n            [Position7] => Mage/Mage\n            [Position8] => Paladin/Paladin\n            [Position9] => \n        )\n\n)\n"
        yield return new WaitForSeconds(1);
        _dbManager.GetUserStats();
    }

    private void GetStatsHandler(UserData userData)
    {
        if (userData.error.isError)
        {
            Debug.Log(userData.error.errorText + "awdaw");
            return;
        }

        Debug.Log(userData.playerData.health + "awdaw");
    }

    private void SetCameraForPlayer(Vector3 _pos, Vector3 _rotat) //метод определяет камеру для игрока в зависимости от его команды
    {
        if(!isLocalPlayer)
        {
            return;
        }

        _cameraTransform.SetPositionAndRotation(_pos, Quaternion.Euler(_rotat));
    }

    private void SetNPCPosition() //раставляем НПС по спавн поинтам сетки согласно заранее запланированным позициям игроком
    {
        if (!isLocalPlayer)
        {
            return;
        }

/*#if UNITY_EDITOR
        GameObject _newNPC = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefab/Paladin/Paladin.prefab", typeof(GameObject));
#else
        GameObject _newNPC = Resources.Load<GameObject>($"Paladin");
#endif*/

        for (int _x = 0; _x < _SpawnSpots.Count; _x++)
        {
            int _y = Random.Range(1, 100);

            switch (_y)
            {
                case <=50:
                    //SpawnNPC(_x, _newNPC, _SpawnSpots[_x]);print(_newNPC);
                    //GameObject _spawnObject = Instantiate(_cube, _SpawnSpots[_x].transform.position, _SpawnSpots[_x].transform.rotation);
                    SpawnNPC(_x);//_spawnObject);
                    break;
                case >50:
                    break;
            }
        }
    }

    [Command]
    private void SpawnNPC(int _x)//, GameObject _NPC, GameObject _Spots)
    {

/*#if UNITY_EDITOR
        GameObject _newNPC = (GameObject)AssetDatabase.LoadAssetAtPath($"Assets/Prefab/Paladin/Paladin.prefab", typeof(GameObject));
#else
        GameObject _newNPC = Resources.Load<GameObject>($"Paladin");
#endif*/
        //List <GameObject> _newNPC = GameObject.Find("NetMan").GetComponent<NetMan>().spawnPrefabs;
        //GameObject _spawnObject = Instantiate(_NPC, _SpawnSpots[_x].transform.position, _SpawnSpots[_x].transform.rotation);
        GameObject _spawnObject = Instantiate(_netRegistredPrefab.Find(_npc => _npc.name == "Paladin"), _SpawnSpots[_x].transform.position, _SpawnSpots[_x].transform.rotation);
        NetworkServer.Spawn(_spawnObject);
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            _NPCGrid.Add(other.gameObject);
            Movement _otherParamMovement = other.gameObject.GetComponent<Movement>();

            _otherParamMovement._gridTeam = _teamNum;
            _otherParamMovement._gridPos = _xPos;
            _otherParamMovement._gridParent = gameObject;
        }
    }

    private void NPCGridRemove(GameObject _name, GameObject _NPCRemove) //метод убирает НПС из листа сетки
    {
        if (_name == gameObject)
        {
            _NPCGrid.Remove(_NPCRemove);
        }
    }

    private void ColliderDisable()//выключаем коллайдер, так как он нужен только на старте для добавления НПС в лист, Invoke в Start
    {
        GetComponent<Collider>().enabled = false;
    }

    private void OnDestroy()
    {
        GameEventManager.GridDestroy -= NPCGridRemove;
        GameObjectsManager.UnregisterGrid(gameObject);
    }

    private void TakePlayerInformation()
    {
        
    }

    #region NPCInfo

    

    #endregion
}
