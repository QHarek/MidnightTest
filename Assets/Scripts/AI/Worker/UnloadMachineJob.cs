using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public sealed class UnloadMachineJob : WorkerJob, ISaveable
{
    [SerializeField] private GameObject _box;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    private Transform _storage;
    private Transform _unloadSpot;
    private InventoryManager _inventoryManager;
    private Machine _machine;
    private Dictionary<string, int> _loot = new Dictionary<string, int>();
    private Vector3 _destination;


    private void Start()
    {
        SaveManager.Instance.unloadMachineJobs.Add(this);
        _inventoryManager = FindObjectOfType<InventoryManager>();
        AddObserver(GetComponent<JobManager>());
        _storage = GameObject.Find("Storage").transform;
    }

    internal override void StartWorking(string machineName = null)
    {
        base.StartWorking();
        _machine = GameObject.Find(machineName).GetComponent<Machine>();
        _unloadSpot = GameObject.Find(machineName + "UnloadSpot").transform;
        StartCoroutine(GoToMachine());
    }

    internal override void StopWorking()
    {
        base.StopWorking();
        StopAllCoroutines();
    }

    private IEnumerator GoToMachine()
    {
        NotifyObservers(WorkerStates.GoingToMachine);
        _animator.Play("WorkerWalking");
        _navMeshAgent.SetDestination(_unloadSpot.position);
        _navMeshAgent.isStopped = false;

        yield return new WaitUntil(() => Vector3.Distance(transform.position, _unloadSpot.position) < 0.1f);
        _loot.AddRange(_machine.ExportProcessedLoot());
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
        UnloadMachineJobData m_data = data as UnloadMachineJobData;
        if (m_data != null)
        {
            _loot = new Dictionary<string, int>();
            _loot.AddRange(m_data.m_loot);
            JobManager jobManager = GetComponent<JobManager>();
            if (jobManager.CurrentJob == Jobs.UnloadCrusher
                || jobManager.CurrentJob == Jobs.UnloadPurifier
                || jobManager.CurrentJob == Jobs.UnloadFusionReactor)
            {
                if (jobManager.WorkerState == WorkerStates.GoingToMachine)
                {
                    StartWorking(m_data.m_machineName);
                }
                else
                {
                    StartCoroutine(CarryBoxToStorage());
                }
            }
            Debug.Log(gameObject.name + "UnloadMachineJob Loaded");
        }
        else
        {
            Debug.LogWarning(gameObject.name + "UnloadMachineJob Load Error");
        }
    }

    public object Save()
    {
        UnloadMachineJobData m_data;
        if (_machine)
        {
            m_data = new UnloadMachineJobData()
            {
                m_loot = new Dictionary<string, int>(),
                m_machineName = _machine.BuildingName,
            };
        }
        else
        {
            m_data = new UnloadMachineJobData()
            {
                m_loot = new Dictionary<string, int>(),
            };
        }

        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class UnloadMachineJobData
{
    public Dictionary<string, int> m_loot;
    public string m_machineName;
}
