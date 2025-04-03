using System.Collections;
using UnityEngine;

public class ShaftMining : MonoBehaviour, ISaveable
{
    [SerializeField] private ShaftUIUpdater _shaftUIUpdater;
    [SerializeField] private MineCart _mineCart;
    [SerializeField] private int _hiringCost;
    [SerializeField] private int _maxMiners;
    [SerializeField] private int _miningSpeedPerWorker;

    private int _minersCount = 1;
    private ShaftContent _shaftContent;

    private void Start()
    {
        SaveManager.Instance.shaftMiningInstance = this;
        _shaftContent = GetComponent<ShaftContent>();
        StartCoroutine(StartMining());
    }

    private IEnumerator StartMining()
    {
        while (true) 
        {
            yield return new WaitForSeconds(1 / _miningSpeedPerWorker);
            _mineCart.AddLoot(_shaftContent.GetRandomCrystal(), 1);
        }
    }

    public void HireWorker()
    {
        if (_minersCount != _maxMiners)
        {
            _minersCount++;
            FindObjectOfType<BalanceManager>().AddRegularExpediture(10, "Miners");
            FindObjectOfType<BalanceManager>().RemoveMoney(100, null);
            StartCoroutine(StartMining());
            _shaftUIUpdater.UpdateShaftUIWorkers(_minersCount);
        }
    }

    public void Load(object data)
    {
        ShaftMiningData m_data = data as ShaftMiningData;
        if (m_data != null)
        {
            for (int i = 1; i < m_data.MinersCount; i++)
            {
                HireWorker();
            }
            Debug.Log("ShaftMining Loaded");
        }
        else
        {
            Debug.LogWarning("ShaftMining Load Error");
        }
    }

    public object Save()
    {
        return new ShaftMiningData()
        {
            MinersCount = _minersCount,
        };
    }
}


[System.Serializable]
public class ShaftMiningData
{
    public int MinersCount;
}
