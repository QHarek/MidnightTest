using System.Collections.Generic;
using UnityEngine;

public class WorkerHireManager : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject _worker;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private TMPro.TextMeshProUGUI _amountText;
    [SerializeField] private int _maxAmount;

    private int _workersAmount = 0;
    private List<GameObject> _workers = new List<GameObject>();

    private void Start()
    {
        Hire();
    }

    public void Hire()
    {
        if (_workersAmount != _maxAmount)
        {
            _workers.Add(Instantiate(_worker, new Vector3(0, 0, -27), Quaternion.identity, _spawnPoint));
            FindObjectOfType<BalanceManager>().AddRegularExpediture(20, "Workers");
            FindObjectOfType<BalanceManager>().RemoveMoney(200, null);
            _workersAmount++;
            _amountText.text = _workersAmount.ToString();
        }
    }

    public void Load(object data)
    {
        WorkerHireManagerData m_data = data as WorkerHireManagerData;
        if (m_data != null)
        {
            _workersAmount = m_data.MinersCount;
            for (int i = 0; i < _workers.Count; i++)
            {
                Destroy(_workers[i]);
            }
            _workers.Clear();
            for (int i = 0; i < _workersAmount; i++)
            {
                Hire();
                //_workers[i].GetComponent<JobManager>().Load(LoadManager.gameData.jobManagerDataList[i]);
                //_workers[i].GetComponent<UnloadCartJob>().Load(LoadManager.gameData.unloadCartJobDataList[i]);
                //_workers[i].GetComponent<UnloadMachineJob>().Load(LoadManager.gameData.unloadMachineJobDataList[i]);
                //_workers[i].GetComponent<LoadTrainJob>().Load(LoadManager.gameData.loadTrainJobDataList[i]);
                //_workers[i].GetComponent<LoadMachineJob>().Load(LoadManager.gameData.loadMachineJobDataList[i]);
            }
            Debug.Log("WorkerHireManagerData Loaded");
        }
        else
        {
            Debug.LogWarning("WorkerHireManagerData Load Error");
        }
    }

    public object Save()
    {
        return new WorkerHireManagerData()
        {
            MinersCount = _workersAmount,
        };
    }
}

[System.Serializable]
public class WorkerHireManagerData
{
    public int MinersCount;
}
