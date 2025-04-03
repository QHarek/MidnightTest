using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum Jobs
{
    UnloadCart,
    UnloadCrusher,
    LoadCrusher,
    UnloadPurifier,
    LoadPurifier,
    UnloadFusionReactor,
    LoadFusionReactor,
    LoadTrain,
    Idle,
}

public class JobManager : MonoBehaviour, IWorkerStateObserver, IBuildingManagerObserver, ISaveable
{
    [SerializeField] private List<Jobs> _jobList;
    [SerializeField] private List<WorkerJob> _workerJobs;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _navMeshAgent;

    [SerializeField] private Jobs _currentJob;
    [SerializeField] private WorkerStates _workerState;

    private List<Machine> _machines = new List<Machine>();
    private MineCart _mineCart;
    private Train _train;
    private JobStateMachine _stateMachine;
    private InventoryManager _inventoryManager;

    public WorkerStates WorkerState => _workerState;
    public Jobs CurrentJob => _currentJob;

    private void Start()
    {
        SaveManager.Instance.jobManagers.Add(this);
        BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
        buildingManager.AddObserver(this);
        _machines.AddRange(buildingManager.BuiltMachines);
        _mineCart = FindObjectOfType<MineCart>();
        _train = FindObjectOfType<Train>();
        _inventoryManager = FindObjectOfType<InventoryManager>();
        _currentJob = Jobs.Idle;
        _workerState = WorkerStates.Idle;
        _stateMachine = new JobStateMachine(this, _navMeshAgent, _workerJobs, _animator);
        FindNewJob();
    }

    private void Update()
    {
        if(_workerState == WorkerStates.Idle)
        {
            FindNewJob();
        }
        _stateMachine.Update();
    }

    private void FindNewJob()
    {
        if (_mineCart.IsAvailableToUnload && !_inventoryManager.IsFull && WorkerJob.JobsAndWorkers[Jobs.UnloadCart] == 0)
        {
            _currentJob = Jobs.UnloadCart;
            WorkerJob.JobsAndWorkers[_currentJob]++;
            return;
        }

        foreach (var machine in _machines)
        {
            switch (machine.BuildingName)
            {
                case "Crusher":
                    if (machine.ProcessingProgress == 100 && !_inventoryManager.IsFull
                        && WorkerJob.JobsAndWorkers[Jobs.UnloadCrusher] == 0)
                    {
                        _currentJob = Jobs.UnloadCrusher;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                        return;
                    }
                    else if(IsAbleToProcess(machine) 
                        && WorkerJob.JobsAndWorkers[Jobs.LoadCrusher] == 0)
                    {
                        _currentJob= Jobs.LoadCrusher;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                        return;
                    }
                    break;
                case "Purifier":
                    if (machine.ProcessingProgress == 100 && !_inventoryManager.IsFull
                        && WorkerJob.JobsAndWorkers[Jobs.UnloadPurifier] == 0)
                    {
                        _currentJob = Jobs.UnloadPurifier;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                        return;
                    }
                    else if (IsAbleToProcess(machine) 
                        && WorkerJob.JobsAndWorkers[Jobs.LoadPurifier] == 0)
                    {
                        _currentJob = Jobs.LoadPurifier;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                        return;
                    }
                    break;
                case "FusionReactor":
                    if (machine.ProcessingProgress == 100 && !_inventoryManager.IsFull
                        && WorkerJob.JobsAndWorkers[Jobs.UnloadFusionReactor] == 0)
                    {
                        _currentJob = Jobs.UnloadFusionReactor;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                    }
                    else if (IsAbleToProcess(machine) 
                        && WorkerJob.JobsAndWorkers[Jobs.LoadFusionReactor] == 0)
                    {
                        _currentJob = Jobs.LoadFusionReactor;
                        WorkerJob.JobsAndWorkers[_currentJob]++;
                    }
                    break;
            }
        }

        if(_inventoryManager.CurrentCapacity > 0 && _currentJob == Jobs.Idle 
            && _train.IsAvailableToLoad)
        {
            _currentJob = Jobs.LoadTrain;
            WorkerJob.JobsAndWorkers[_currentJob]++;
        }
    }

    public void OnWorkerStateChanged(WorkerStates newState)
    {
        _workerState = newState;
        if (_workerState == WorkerStates.Idle)
        {
            WorkerJob.JobsAndWorkers[_currentJob]--;
            _currentJob = Jobs.Idle;
        }
    }

    public void OnBuildingAction(BuildingAction action, Machine building)
    {
        if(action == BuildingAction.Build && !_machines.Contains(building))
        {
            _machines.Add(building);
        }
    }

    private bool IsAbleToProcess(Machine machine)
    {
        if (_inventoryManager.CheckAvailableResources(machine.RequieredResources) && machine.ProcessingProgress == 0)
        {
            return true;
        }
        return false;
    }

    public void Load(object data)
    {
        JobManagerData m_data = data as JobManagerData;
        if (m_data != null)
        {
            _currentJob = m_data.m_currentJob;
            _workerState = m_data.m_workerState;
            transform.position = m_data.m_position.ToVector3();
            Debug.Log(gameObject.name + " JobManager Loaded");
        }
        else
        {
            Debug.LogWarning(gameObject.name + "JobManager Load Error");
        }
    }

    public object Save()
    {
        return new JobManagerData()
        {
            m_currentJob = _currentJob,
            m_workerState = _workerState,
            m_position = new SerializableVector3(transform.position),
        };
    }
}

[System.Serializable]
public class JobManagerData
{
    public Jobs m_currentJob;
    public WorkerStates m_workerState;
    public SerializableVector3 m_position;
}
