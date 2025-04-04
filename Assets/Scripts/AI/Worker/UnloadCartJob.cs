using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public sealed class UnloadCartJob : WorkerJob, ISaveable
{
    [SerializeField] private GameObject _box;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Transform _storage;
    private Transform _unloadSpot;
    private InventoryManager _inventoryManager;
    private MineCart _mineCart;
    private Vector3 _destination;
    private Dictionary<string, int> _loot = new Dictionary<string, int>();


    private void Start()
    {
        SaveManager.Instance.unloadCartJobs.Add(this);
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _mineCart = FindObjectOfType<MineCart>();
        AddObserver(GetComponent<JobManager>());
        _storage = GameObject.Find("Storage").transform;
        _unloadSpot = GameObject.Find("UnloadSpot").transform;
    }

    internal override void StartWorking(string machine = null)
    {
        base.StartWorking();
        StartCoroutine(GoToCart());
    }

    internal override void StopWorking()
    {
        base.StopWorking();
        StopAllCoroutines();
    }

    private IEnumerator GoToCart()
    {
        NotifyObservers(WorkerStates.GoingToCart);
        _animator.Play("WorkerWalking");
        _navMeshAgent.SetDestination(_unloadSpot.position);
        _navMeshAgent.isStopped = false;
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _unloadSpot.position) < 0.1f);
        while (_mineCart.IsAvailableToUnload)
        {
            _loot = _mineCart.UnloadLoot();
        }
        StartCoroutine(CarryBoxToStorage());
    }

    private IEnumerator CarryBoxToStorage()
    {
        NotifyObservers(WorkerStates.GoingToStorage);
        _animator.Play("WorkerCarryBoxWalk");
         _destination = GetClosestStorageUnit();
        _navMeshAgent.SetDestination(_destination);
        _navMeshAgent.isStopped = false;
        _box.SetActive(true);
        yield return new WaitUntil(() => Vector3.Distance(transform.position, _destination) < 0.8f);

        foreach (var item in _loot)
        {
            _inventoryManager.ImportItem(item.Key, item.Value);
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
        UnloadCartJobData m_data = data as UnloadCartJobData;
        if (m_data != null)
        {
            _loot = new Dictionary<string, int>();
            _loot.AddRange(m_data.m_loot);
            JobManager jobManager = GetComponent<JobManager>();
            if (jobManager.CurrentJob == Jobs.UnloadCart)
            {
                if (jobManager.WorkerState == WorkerStates.GoingToCart)
                {
                    StartWorking();
                }
                else
                {
                    StartCoroutine(CarryBoxToStorage());
                }
            }
            Debug.Log(gameObject.name + "UnloadCartJob Loaded");
        }
        else
        {
            Debug.LogWarning(gameObject.name + "UnloadCartJob Load Error");
        }
    }

    public object Save()
    {
        UnloadCartJobData m_data = new UnloadCartJobData()
        {
            m_loot = new Dictionary<string, int>(),
        };
        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class UnloadCartJobData
{
    public Dictionary<string, int> m_loot;
}