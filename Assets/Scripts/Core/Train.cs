using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Train : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject _visibleLoot;
    [SerializeField] private EconomicsSettings _ecomonicsSettings;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _pathTime;

    private Dictionary<string, int> _loot = new Dictionary<string, int>()    {
        {"CrystalCommon", 0 },
        {"CrystalUncommon", 0 },
        {"CrystalRare", 0 },
        {"CrystalEpic", 0 },
        {"DustCommon", 0 },
        {"DustUncommon", 0 },
        {"DustRare", 0 },
        {"DustEpic", 0 },
    };
    private BalanceManager _balanceManager;
    private bool _isAvailableToLoad = false;
    private bool _isLoaded = false;

    public bool IsAvailableToLoad => _isAvailableToLoad;

    private void Start()
    {
        SaveManager.Instance.trainInstance = this;
        StartCoroutine(MoveToWareHouse());
        _balanceManager = FindObjectOfType<BalanceManager>();
    }

    private IEnumerator MoveToWareHouse()
    {
        _visibleLoot.SetActive(false);
        _animator.Play("TrainToWarehouse");
        yield return new WaitForSeconds(_pathTime);
        if (!_isLoaded)
        {
            SellLoot();
            _loot.Clear();
        }
        else
        {
            _isLoaded = false;
        }
        _isAvailableToLoad = true;
        StartCoroutine(MoveToSell());
    }

    private IEnumerator MoveToSell()
    {
        yield return new WaitForSeconds(60);
        _isAvailableToLoad = false;
        yield return new WaitUntil(() => WorkerJob.JobsAndWorkers[Jobs.LoadTrain] == 0);
        _animator.Play("TrainToSell");
        yield return new WaitForSeconds(120);
        StartCoroutine(MoveToWareHouse());
    }

    public void AddLoot(string lootName, int lootCount)
    {
        if (_loot.ContainsKey(lootName))
        {
            _loot[lootName] += lootCount;
        }
        else
        {
            _loot.Add(lootName, lootCount);
        }
        _visibleLoot.SetActive(true);
    }

    private void SellLoot()
    {
        if (_loot.ContainsKey("CrystalCommon"))
        {
            _loot["CrystalCommon"] *= _ecomonicsSettings.commonCrystalValue;
        }
        if (_loot.ContainsKey("CrystalUncommon"))
        {
            _loot["CrystalUncommon"] *= _ecomonicsSettings.uncommonCrystalValue;
        }
        if (_loot.ContainsKey("CrystalRare"))
        {
            _loot["CrystalRare"] *= _ecomonicsSettings.rareCrystalValue;
        }
        if (_loot.ContainsKey("CrystalEpic"))
        {
            _loot["CrystalEpic"] *= _ecomonicsSettings.epicCrystalValue;
        }
        if (_loot.ContainsKey("DustCommon"))
        {
            _loot["DustCommon"] *= _ecomonicsSettings.commonDustValue;
        }
        if (_loot.ContainsKey("DustUncommon"))
        {
            _loot["DustUncommon"] *= _ecomonicsSettings.uncommonDustValue;
        }
        if (_loot.ContainsKey("DustRare"))
        {
            _loot["DustRare"] *= _ecomonicsSettings.rareDustValue;
        }
        if (_loot.ContainsKey("DustEpic"))
        {
            _loot["DustEpic"] *= _ecomonicsSettings.epicDustValue;
        }

        _balanceManager.AddMoney(_loot);
    }

    public void Load(object data)
    {
        StopAllCoroutines();
        TrainData m_data = data as TrainData;
        if (m_data != null)
        {
            _loot.Clear();
            _loot.AddRange(m_data.m_loot);
            transform.position = m_data.m_position.ToVector3();
            _isAvailableToLoad = m_data.m_isAvailableToLoad;
            if (_isAvailableToLoad)
            {
                StartCoroutine(MoveToWareHouse());
                _isLoaded = true;
            }
            else
            {
                StartCoroutine(MoveToSell());
            }
            Debug.Log("Train Loaded");
        }
        else
        {
            Debug.LogWarning("Train Load Error");
        }
    }

    public object Save()
    {
        TrainData m_data = new TrainData()
        {
            m_loot = new Dictionary<string, int>(),
            m_position = new SerializableVector3(transform.position),
            m_isAvailableToLoad = _isAvailableToLoad
        };
        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class TrainData
{
    public Dictionary<string, int> m_loot;
    public SerializableVector3 m_position;
    public bool m_isAvailableToLoad;
}
