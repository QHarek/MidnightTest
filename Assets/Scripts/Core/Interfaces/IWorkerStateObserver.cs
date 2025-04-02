public enum WorkerStates
{
    Idle,
    GoingToStorage,
    GoingToTrain,
    GoingToMachine,
    GoingToCart,
}

public interface IWorkerStateObserver
{
    void OnWorkerStateChanged(WorkerStates newState);
}
