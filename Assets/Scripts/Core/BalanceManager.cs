using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BalanceManager : MonoBehaviour, ISaveable
{
    [SerializeField] private BalanceUIUpdate _balanceUIUpdate;

    private float _lastCalculateTime;
    private float _calculatePeriod = 60;
    private int _balance = 10000;
    private bool _isLoaded = false;

    private Dictionary<string, int> _incomeList = new Dictionary<string, int>()    {
        {"CrystalCommon", 0 },
        {"CrystalUncommon", 0 },
        {"CrystalRare", 0 },
        {"CrystalEpic", 0 },
        {"DustCommon", 0 },
        {"DustUncommon", 0 },
        {"DustRare", 0 },
        {"DustEpic", 0 },
    };
    private Dictionary<string, int> _expeditureList = new Dictionary<string, int>()    {
        {"Rent", 0 },
        {"Miners", 0 },
        {"Workers", 0 },
        {"Machines", 0 }
    };
    private Dictionary<string, int> _regularExpeditureList = new Dictionary<string, int>()    {
        {"Rent", 200 },
        {"Miners", 10 },
        {"Workers", 20 },
        {"Machines", 0 }
    };

    public int Balance => _balance;

    private void Start()
    {
        SaveManager.Instance.balanceManagerInstance = this;
        if (!_isLoaded)
        {
            CalculateExpeditures();
        }
        else
        {
            _isLoaded = true;
        }
    }

    private void Update()
    {
        if (Time.time - _lastCalculateTime >= _calculatePeriod)
        {
            CalculateExpeditures();
        }

        if (_balance < -5000)
        {

        }
    }

    public void AddMoney(Dictionary<string, int> lootValue)
    {
        foreach (var item in lootValue)
        {
            _balance += item.Value;
            _incomeList[item.Key] += item.Value;
        }
        _balanceUIUpdate.UpdateBalanceUI("income", _incomeList);
    }

    public void RemoveMoney(int amount, string source) 
    {
        _balance -= amount;
    }

    public void AddRegularExpediture(int amount, string source)
    {
        _regularExpeditureList[source] += amount;
        _balanceUIUpdate.UpdateBalanceUI("expediture", _regularExpeditureList);
    }

    public void RemoveRegularExpediture(int amount, string source)
    {
        _regularExpeditureList[source] -= amount;
        _balanceUIUpdate.UpdateBalanceUI("expediture", _regularExpeditureList);
    }

    public void ClearLists()
    {
        _expeditureList = new Dictionary<string, int>()    
        {
            {"Rent", 0 },
            {"Miners", 0 },
            {"Workers", 0 },
            {"Machines", 0 }
        };

        _incomeList = new Dictionary<string, int>()
        {
            {"CrystalCommon", 0 },
            {"CrystalUncommon", 0 },
            {"CrystalRare", 0 },
            {"CrystalEpic", 0 },
            {"DustCommon", 0 },
            {"DustUncommon", 0 },
            {"DustRare", 0 },
            {"DustEpic", 0 },
        };
    }

    private void CalculateExpeditures()
    {
        foreach (var item in _regularExpeditureList)
        {
            _expeditureList[item.Key] += item.Value;
            _balance -= item.Value;
        }
        _balanceUIUpdate.UpdateBalanceUI("expediture", _regularExpeditureList);
        _lastCalculateTime = Time.time;
    }

    public void Load(object data)
    {
        BalanceManagerData m_data = data as BalanceManagerData;
        if (m_data != null)
        {
            _lastCalculateTime = Time.time - m_data.m_lastCalculateTime;
            _incomeList.Clear();
            _incomeList.AddRange(m_data.m_incomeList);
            _expeditureList.Clear();
            _expeditureList.AddRange(m_data.m_expeditureList);
            _regularExpeditureList.Clear();
            _regularExpeditureList.AddRange(m_data.m_regularExpeditureList);
            _balance = m_data.m_balance;
            _balanceUIUpdate.UpdateBalanceUI("expediture", _regularExpeditureList);
            _balanceUIUpdate.UpdateBalanceUI("income", _incomeList);
            _isLoaded = true;
            Debug.Log("BalanceManager Loaded");
        }
        else
        {
            Debug.LogWarning("BalanceManager Load Error");
        }
    }

    public object Save()
    {
        BalanceManagerData m_data = new BalanceManagerData()
        {
            m_lastCalculateTime = Time.time - _lastCalculateTime,
            m_balance = _balance,
            m_incomeList = new Dictionary<string, int>(),
            m_expeditureList = new Dictionary<string, int>(),
            m_regularExpeditureList = new Dictionary<string, int>(),
        };
        m_data.m_incomeList.AddRange(_incomeList);
        m_data.m_expeditureList.AddRange(_expeditureList);
        m_data.m_regularExpeditureList.AddRange(_regularExpeditureList);
        return m_data;
    }
}

[System.Serializable]
public class BalanceManagerData
{
    public float m_lastCalculateTime;
    public int m_balance;
    public Dictionary<string, int> m_incomeList;
    public Dictionary<string, int> m_expeditureList;
    public Dictionary<string, int> m_regularExpeditureList;
}
