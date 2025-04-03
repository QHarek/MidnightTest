using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public sealed class LoadMachineJob : WorkerJob, ISaveable
{
    [SerializeField] private GameObject _box;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Transform _storage;
    private Transform _loadSpot;
    private InventoryManager _inventoryManager;
    private Machine _machine;
    private Dictionary<string, int> _loot = new Dictionary<string, int>();
    private int _currentCapacity;
    private int _maxCapacity = 50;
    private bool _isLootEmpty = false;
    private Vector3 _destination;

    private void Start()
    {
        SaveManager.Instance.loadMachineJobs.Add(this);
        _inventoryManager = FindObjectOfType<InventoryManager>();
        AddObserver(GetComponent<JobManager>());
        _storage = GameObject.Find("Storage").transform;
    }

    internal override void StartWorking(string? machineName = null)
    {
        base.StartWorking();
        _machine = GameObject.Find(machineName).GetComponent<Machine>();
        _loadSpot = GameObject.Find(machineName + "LoadSpot").transform;
        StartCoroutine(GoToStorage());
    }

    internal override void StopWorking()
    {
        base.StopWorking();
        _loot.Clear();
    }

    private IEnumerator GoToStorage()
    {
        NotifyObservers(WorkerStates.GoingToStorage);
        _animator.Play("WorkerWalking");
        _destination = GetClosestStorageUnit();
        _navMeshAgent.SetDestination(_destination);
        _navMeshAgent.isStopped = false;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _destination) < 0.8f);
        foreach (string resource in _machine.RequieredResources)
        {
            if (_currentCapacity < _maxCapacity)
            {
                int amount = 0;
                int itemCount = _inventoryManager.GetAvailableItemAmount(resource);
                if (itemCount > _maxCapacity - _currentCapacity)
                {
                    amount = _maxCapacity - _currentCapacity;
                }
                else
                {
                    amount = itemCount;
                }
                amount = _inventoryManager.ExportRecycleItem(resource, amount);
                if (_loot.ContainsKey(resource) && amount > 0)
                {
                    _loot[resource] += amount;
                }
                else
                {
                    _loot.Add(resource, amount);
                }
                _currentCapacity += amount;
            }
            else
            {
                break;
            }
        }

        foreach (var item in _loot)
        {
            if (item.Value != 0)
            {
                _isLootEmpty = false;
                break;
            }
            _isLootEmpty = true;
        }

        if (_isLootEmpty)
        {
            StopWorking();
        }
        else
        {
            StartCoroutine(CarryBoxToMachine());
        }
    }

    private IEnumerator CarryBoxToMachine()
    {
        NotifyObservers(WorkerStates.GoingToMachine);
        _animator.Play("WorkerCarryBoxWalk");
        _navMeshAgent.SetDestination(_loadSpot.position);
        _navMeshAgent.isStopped = false;
        _box.SetActive(true);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _loadSpot.position) < 0.1f);

        _machine.ImportRawLoot(_loot);
        _currentCapacity = 0;
        _loot.Clear();
        _box.SetActive(false);
        StopWorking();
    }

    private Vector3 GetClosestStorageUnit()
    {
        Transform closestUnit = _storage.GetChild(0);
        float distance = Vector3.Distance(closestUnit.position, transform.position);

        for (int i = 0; i < _storage.childCount; i++)
        {
            if (_storage.GetChild(i).gameObject.activeSelf)
            {
                if (Vector3.Distance(_storage.GetChild(i).position, transform.position) < distance)
                {
                    closestUnit = _storage.GetChild(i);
                }
            }
        }

        return closestUnit.GetComponent<Collider>().ClosestPoint(transform.position);
    }

    public void Load(object data)
    {
        LoadMachineJobData m_data = data as LoadMachineJobData;
        if (m_data != null)
        {
            _isLootEmpty = m_data.m_isLootEmpty;
            _currentCapacity = m_data.m_currentCapacity;
            _loot = new Dictionary<string, int>();
            _loot.AddRange(m_data.m_loot);
            JobManager jobManager = GetComponent<JobManager>();

            if (jobManager.CurrentJob == Jobs.LoadCrusher
                || jobManager.CurrentJob == Jobs.LoadPurifier
                || jobManager.CurrentJob == Jobs.LoadFusionReactor)
            {
                if (jobManager.WorkerState != WorkerStates.GoingToMachine)
                {
                    StartWorking(m_data.m_machineName);
                }
                else
                {
                    _machine = GameObject.Find(m_data.m_machineName).GetComponent<Machine>();
                    _loadSpot = GameObject.Find(m_data.m_machineName + "LoadSpot").transform;
                    StartCoroutine(CarryBoxToMachine());
                }
            }
            Debug.Log(gameObject.name + "LoadMachineJob Loaded");
        }
        else
        {
            Debug.LogWarning(gameObject.name + "LoadMachineJob Load Error");
        }
    }

    public object Save()
    {
        LoadMachineJobData m_data;
        if (_machine)
        {
            m_data = new LoadMachineJobData()
            {
                m_loot = new Dictionary<string, int>(),
                m_machineName = _machine.BuildingName,
                m_currentCapacity = _currentCapacity,
                m_isLootEmpty = _isLootEmpty,
            };
        }
        else
        {
            m_data = new LoadMachineJobData()
            {
                m_loot = new Dictionary<string, int>(),
                m_currentCapacity = _currentCapacity,
                m_isLootEmpty = _isLootEmpty,
            };
        }

        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class LoadMachineJobData
{
    public Dictionary<string, int> m_loot;
    public string m_machineName;
    public int m_currentCapacity;
    public bool m_isLootEmpty;
}
