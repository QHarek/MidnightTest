using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public sealed class LoadTrainJob : WorkerJob, ISaveable
{
    [SerializeField] private GameObject _box;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Transform _storage;
    private Transform _loadSpot;
    private InventoryManager _inventoryManager;
    private Train _train;
    private Dictionary<string, int> _loot = new Dictionary<string, int>();
    private int _currentCapacity;
    private int _maxCapacity = 50;
    private bool _isLootEmpty = false;

    private void Start()
    {
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _train = FindObjectOfType<Train>();
        AddObserver(GetComponent<JobManager>());
        _storage = GameObject.Find("Storage").transform;
        _loadSpot = GameObject.Find("LoadSpot").transform;
    }

    internal override void StartWorking(string? machine = null)
    {
        base.StartWorking();
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
        Vector3 destination = GetClosestStorageUnit();
        _navMeshAgent.SetDestination(destination);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, destination) < 0.8f);
        for (int i = _inventoryManager.PublicItems.Count - 1; i >= 0; i--)
        {
            int amount = 0;
            if (_currentCapacity < _maxCapacity)
            {
                string itemName = _inventoryManager.PublicItems.ElementAt(i).Key;
                int itemCount = _inventoryManager.GetAvailableItemAmount(itemName);
                if (itemCount > _maxCapacity - _currentCapacity)
                {
                    amount = _maxCapacity - _currentCapacity;
                }
                else
                {
                    amount = itemCount;
                }

                _inventoryManager.ExportItem(itemName, amount);
                if (_loot.ContainsKey(itemName) && amount > 0)
                {
                    _loot[itemName] += amount;
                }
                else
                {
                    _loot.Add(itemName, amount);
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
            StartCoroutine(CarryBoxToTrain());
        }
    }

    private IEnumerator CarryBoxToTrain()
    {
        NotifyObservers(WorkerStates.GoingToTrain);
        _animator.Play("WorkerCarryBoxWalk");
        _navMeshAgent.SetDestination(_loadSpot.position);
        _box.SetActive(true);

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _loadSpot.position) < 0.1f);

        foreach (var item in _loot)
        {
            _train.AddLoot(item.Key, item.Value);
            _currentCapacity -= item.Value;
        }

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
        LoadTrainJobData m_data = data as LoadTrainJobData;
        if (m_data != null)
        {
            _isLootEmpty = m_data.m_isLootEmpty;
            _currentCapacity = m_data.m_currentCapacity;
            _loot.Clear();
            _loot.AddRange(m_data.m_loot);
            JobManager jobManager = GetComponent<JobManager>();
            if (jobManager.CurrentJob == Jobs.LoadTrain)
            {
                if (jobManager.WorkerState == WorkerStates.GoingToStorage)
                {
                    StartWorking();
                }
                else
                {
                    StartCoroutine(CarryBoxToTrain());
                }
            }
            Debug.Log(gameObject.name + "LoadTrainJob Loaded");
        }
        else
        {
            Debug.LogWarning(gameObject.name + "LoadTrainJob Load Error");
        }
    }

    public object Save()
    {
        LoadTrainJobData m_data = new LoadTrainJobData()
        {
            m_loot = new Dictionary<string, int>(),
            m_currentCapacity = _currentCapacity,
            m_isLootEmpty = _isLootEmpty,
        };
        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class LoadTrainJobData
{
    public Dictionary<string, int> m_loot;
    public int m_currentCapacity;
    public bool m_isLootEmpty;
}