using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorkerHireManager : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject _worker;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private TMPro.TextMeshProUGUI _amountText;
    [SerializeField] private int _maxAmount;

    private List<GameObject> _workers = new List<GameObject>();

    private void Start()
    {
        SaveManager.Instance.workerHireManagerInstance = this;
    }

    public void Hire()
    {
        if (_workers.Count != _maxAmount)
        {
            _workers.Add(Instantiate(_worker, new Vector3(0, 0, -27), Quaternion.identity, _spawnPoint));
            FindObjectOfType<BalanceManager>().AddRegularExpediture(20, "Workers");
            FindObjectOfType<BalanceManager>().RemoveMoney(200, null);
            _amountText.text = _workers.Count.ToString();
        }
    }

    public void Load(object data)
    {
        WorkerHireManagerData m_data = data as WorkerHireManagerData;
        if (m_data != null)
        {
            for (int i = 0; i < _workers.Count; i++)
            {
                Destroy(_workers[i]);
            }
            _workers.Clear();
            for (int i = 0; i < m_data.m_workersAmount; i++)
            {
                Hire();
                StartCoroutine(LoadComponents(i));
            }
            Debug.Log("WorkerHireManagerData Loaded");
        }
        else
        {
            Debug.LogWarning("WorkerHireManagerData Load Error");
        }
    }

    private IEnumerator LoadComponents(int index)
    {
        JobManager jobManager = null;
        yield return new WaitUntil(() => _workers[index].TryGetComponent(out jobManager));
        jobManager.Load(LoadManager.gameData.jobManagerDataList[index]);

        UnloadCartJob unloadCartJob = null;
        yield return new WaitUntil(() => _workers[index].TryGetComponent(out unloadCartJob));
        unloadCartJob.Load(LoadManager.gameData.unloadCartJobDataList[index]);

        UnloadMachineJob unloadMachineJob = null;
        yield return new WaitUntil(() => _workers[index].TryGetComponent(out unloadMachineJob));
        unloadMachineJob.Load(LoadManager.gameData.unloadMachineJobDataList[index]);

        LoadTrainJob loadTrainJob = null;
        yield return new WaitUntil(() => _workers[index].TryGetComponent(out loadTrainJob));
        loadTrainJob.Load(LoadManager.gameData.loadTrainJobDataList[index]);

        LoadMachineJob loadMachineJob = null;
        yield return new WaitUntil(() => _workers[index].TryGetComponent(out loadMachineJob));
        loadMachineJob.Load(LoadManager.gameData.loadMachineJobDataList[index]);
    }

    public object Save()
    {
        return new WorkerHireManagerData()
        {
            m_workersAmount = _workers.Count,
        };
    }
}

[System.Serializable]
public class WorkerHireManagerData
{
    public int m_workersAmount;
}
