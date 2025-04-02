using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class JobStateMachine
{
    private JobManager _jobManager;
    private NavMeshAgent _navMeshAgent;
    private List<WorkerJob> _workerJobs;
    private Animator _animator;


    public JobStateMachine(JobManager jobManager, NavMeshAgent agent, List<WorkerJob> workerJobs, Animator animator)
    {
        _jobManager = jobManager;
        _navMeshAgent = agent;
        _workerJobs = workerJobs;
        _animator = animator;
    }

    public void Update()
    {
        switch (_jobManager.CurrentJob)
        {
            case Jobs.Idle:
                HandleIdleState();
                break;
            case Jobs.UnloadCart:
                HandleUnloadCartState();
                break;
            case Jobs.UnloadCrusher:
                HandleUnloadMachineState("Crusher");
                break;
            case Jobs.LoadCrusher:
                HandleLoadMachineState("Crusher");
                break;
            case Jobs.UnloadPurifier:
                HandleUnloadMachineState("Purifier");
                break;
            case Jobs.LoadPurifier:
                HandleLoadMachineState("Purifier");
                break;
            case Jobs.UnloadFusionReactor:
                HandleUnloadMachineState("FusionReactor");
                break;
            case Jobs.LoadFusionReactor:
                HandleLoadMachineState("FusionReactor");
                break;
            case Jobs.LoadTrain:
                HandleLoadTrainState();
                break;
            default:
                break;
        }
    }

    private void HandleIdleState()
    {
        _navMeshAgent.isStopped = true;
        _animator.Play("WorkerIdle");
    }

    private void HandleUnloadCartState()
    {
        if (_jobManager.WorkerState == WorkerStates.Idle)
        {
            _navMeshAgent.isStopped = false;
            _workerJobs.Where(m => m.ToString().Contains(_jobManager.CurrentJob.ToString())).First().StartWorking();
        }
    }

    private void HandleUnloadMachineState(string machineName)
    {
        if (_jobManager.WorkerState == WorkerStates.Idle)
        {
            _navMeshAgent.isStopped = false;
            _workerJobs.Where(m => m.ToString().Contains("UnloadMachine")).First().StartWorking(machineName);
        }
    }

    private void HandleLoadMachineState(string machineName)
    {
        if (_jobManager.WorkerState == WorkerStates.Idle)
        {
            _navMeshAgent.isStopped = false;
            _workerJobs.Where(m => m.ToString().Contains("LoadMachine")).First().StartWorking(machineName);
        }
    }

    private void HandleLoadTrainState()
    {
        if (_jobManager.WorkerState == WorkerStates.Idle)
        {
            _navMeshAgent.isStopped = false;
            _workerJobs.Where(m => m.ToString().Contains(_jobManager.CurrentJob.ToString())).First().StartWorking();
        }
    }
}