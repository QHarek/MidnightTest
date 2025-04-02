using System.Collections.Generic;
using UnityEngine;

public abstract class WorkerJob : MonoBehaviour
{
    public static Dictionary<Jobs, int> JobsAndWorkers = new Dictionary<Jobs, int>() 
    {
        {Jobs.Idle, 0},
        {Jobs.UnloadCart, 0},
        {Jobs.UnloadCrusher, 0},
        {Jobs.UnloadPurifier, 0},
        {Jobs.UnloadFusionReactor, 0},
        {Jobs.LoadCrusher, 0},
        {Jobs.LoadPurifier, 0},
        {Jobs.LoadFusionReactor, 0},
        {Jobs.LoadTrain, 0},
    };

    private List<IWorkerStateObserver> _observers = new List<IWorkerStateObserver>();

    internal virtual void StartWorking(string? machineName = null)
    {
    }

    internal virtual void StopWorking()
    {
        NotifyObservers(WorkerStates.Idle);
    }

    public void AddObserver(IWorkerStateObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IWorkerStateObserver observer)
    {
        _observers.Remove(observer);
    }

    protected void NotifyObservers(WorkerStates newState)
    {
        foreach (var observer in _observers)
        {
            observer.OnWorkerStateChanged(newState);
        }
    }
}