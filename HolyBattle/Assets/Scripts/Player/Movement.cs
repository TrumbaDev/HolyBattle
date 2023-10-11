using Mirror;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Movement : NetworkBehaviour
{
    private NavMeshAgent _agent;
    private bool _isGameStarted = false;
    private Animator _animator;
    private NPC _classNPC;
    
    [SerializeField] private Transform _agroTransform;
    [SerializeField] private GameObject _agroGameObject;
        
    public string _gridPos, _gridTeam;//gridTeam �������� "Team1" "Team2"
    public GameObject _gridParent;

    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
        _classNPC = GetComponent<NPC>();

        GameEventManager.StartMovementAction += ChangeStartBool;
        _isGameStarted = true;//�������
        Invoke("CheckGridCount", 2f);
    }

    void Update()
    {
        if (!_isGameStarted)
        {
            return;
        }

        if (_agroTransform)
        {
            _agent.SetDestination(_agroTransform.position);
            _animator.SetFloat("Movement", _agent.remainingDistance);
            transform.LookAt(new Vector3(_agroTransform.position.x, _agroTransform.position.y, _agroTransform.position.z));            
        }
        else
        {
            _animator.SetFloat("Movement", 0);
            FindClosestEnemy();
            _classNPC.OnTriggerExitMovement();
            return;
        }

        if(_animator.GetFloat("Movement") > _agent.stoppingDistance + 0.2f)
        {
            _classNPC.OnTriggerExitMovement();
        }
    }
    
    private void FindClosestEnemy() //����� �� ������ ���������� �����, ������� ���� ����� �� ����� �����
    {
        GameObject[] enemy = GameObjectsManager.GetNPCGridByName(_gridTeam, _gridPos);

        float distance = 10000f;
        Vector3 position = transform.position;

        foreach (GameObject go in enemy)
        {
            Vector3 diff = go.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < distance)
            {
                _agroTransform = go.transform;
                distance = curDistance;

                _agroGameObject = go;
            }
        }
    }

    private void CheckGridCount() //����� �� �������� ����������� ����� (�������) � ����� ����������� ������ ����������� ������
    {
        if (GameObjectsManager.TakeGridCount() > 1) //������ (3) ����� �������� �� ����������, ������� ������� �� ������� ������� ��������� ���� ���
        {
            Invoke("StartEventInitialize", 5f);
        }
    }

    private void StartEventInitialize()
    {
        GameEventManager.StartMovementAction?.Invoke();
    }

    private void ChangeStartBool() //������ ��� true ��������� ��� ������
    {
        _isGameStarted = true;
        GameEventManager.StartMovementAction -= ChangeStartBool;
    }

    private void OnDestroy()
    {
        GameEventManager.StartMovementAction -= ChangeStartBool;
        GameEventManager.GridDestroy?.Invoke(_gridParent, gameObject);
    }

    private void OnApplicationQuit()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent(out Movement _otherMove);

        if (!_otherMove)
        {
            return;
        }
        
        bool _isEqualDistance = _agent.remainingDistance == _agent.stoppingDistance;
        bool _isEqualTeam = _otherMove._gridTeam == _gridTeam;

        if (_isEqualTeam && _isEqualDistance)
        {
            _agent.avoidancePriority += 1;
        }
        else if (_isEqualTeam && !_isEqualDistance)
        {
            _agent.avoidancePriority = 0;
        }

        if (!_isEqualTeam)
        {
            _agroTransform = other.transform;
            _agroGameObject = other.gameObject;
            _classNPC.OnTriggerEnterMovement();
        }
    }

    /*private void OnTriggerExit(Collider other)
    {
        if (_agroGameObject == other.gameObject)
        {
            _classNPC.OnTriggerExitMovement();
        }
    }*/
}
