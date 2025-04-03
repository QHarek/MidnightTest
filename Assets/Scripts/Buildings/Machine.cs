using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Machine : MonoBehaviour, ISaveable, IBuildingManagerObserver
{
    [SerializeField] private string _buildingName;
    [SerializeField] private GameObject _destroyUI;
    
    [SerializeField] protected int _buildingCost;
    [SerializeField] protected int _upgradeCost;
    [SerializeField] protected int _basicBuildingServiceCost;
    [SerializeField] protected int _buildingServiceCostPerUpgrade;
    [SerializeField] protected int _buildingLevel;
    [SerializeField] protected int _maxLevel;
    [SerializeField] protected float _processTime;
    [SerializeField] protected MachineUIUpdate _updateUI;
    [SerializeField] protected List<string> _requieredResources;

    private BuildingSpot _buildingSpot;

    protected Dictionary<string, int> _rawLoot = new Dictionary<string, int>();
    protected Dictionary<string, int> _processedLoot = new Dictionary<string, int>();
    protected bool _isAvailable = true;
    protected int _processingProgress;
    protected int _buildingServiceCost;
    protected float _processingStartTime;

    public string BuildingName => _buildingName;
    public int BuildingCost => _buildingCost;
    public int BuildingServiceCost => _buildingServiceCost;
    public int BuildingLevel => _buildingLevel;
    public float ProcessSpeed => _processTime;
    public int ProcessingProgress => _processingProgress;
    public BuildingSpot BuildingSpot => _buildingSpot;
    public List<string> RequieredResources => _requieredResources;

    private void Awake()
    {
        FindObjectOfType<BuildingManager>().AddObserver(this);
        FindObjectOfType<BalanceManager>().AddRegularExpediture(_basicBuildingServiceCost, "Machines");
        FindObjectOfType<BalanceManager>().RemoveMoney(_buildingCost, null);
        _updateUI = GameObject.Find(_buildingName + "Container").GetComponent<MachineUIUpdate>();
    }

    private void Start()
    {
        SaveManager.Instance.machines.Add(this);
    }

    internal abstract void ProcessLoot(bool isLoaded = false);

    public virtual void Upgrade()
    {
        if (_buildingLevel != _maxLevel)
        {
            _buildingLevel++;
            _buildingServiceCost += _buildingServiceCostPerUpgrade;
            FindObjectOfType<BalanceManager>().AddRegularExpediture(_buildingServiceCostPerUpgrade, "Machines");
            FindObjectOfType<BalanceManager>().RemoveMoney(_upgradeCost, null);
            _updateUI.UpdateLevel(_buildingLevel);
        }
    }

    protected virtual void StopProcessing()
    {
        _processingProgress = 50;
        FindObjectOfType<BalanceManager>().RemoveRegularExpediture(_buildingServiceCost, "Machines");
        _processedLoot.Clear();
        _destroyUI.SetActive(true);
    }

    public void SetBuildingSpot(BuildingSpot spot)
    {
        _buildingSpot = spot;
    }

    public ReadOnlyDictionary<string, int> ExportProcessedLoot()
    {
        if (_isAvailable)
        {
            _processingProgress = 0;
            _updateUI.UpdateProgress(_processingProgress);
        }
        else
        {
            _processingProgress = 50;
        }
        return new ReadOnlyDictionary<string, int>(_processedLoot);
    }

    public void ImportRawLoot(Dictionary<string, int> loot)
    {
        if (_isAvailable)
        {
            foreach (var item in loot)
            {
                if (_rawLoot.ContainsKey(item.Key))
                {
                    _rawLoot[item.Key] += item.Value;
                }
                else
                {
                    _rawLoot.Add(item.Key, item.Value);
                }
            }
            ProcessLoot();
        }
        else
        {
            _processingProgress = 50;
        }
    }

    public void OnBuildingAction(BuildingAction action, Machine building)
    {
        if(action == BuildingAction.Destroy && building.name == name)
        {
            StopProcessing();
            _isAvailable = false;
        }
    }

    public void Load(object data)
    {
        MachineData m_data = data as MachineData;
        if (m_data != null)
        {
            StopAllCoroutines();
            _buildingName = m_data.m_buildingName;
            _buildingSpot = GameObject.Find(m_data.m_buildingSpotName).GetComponent<BuildingSpot>();
            _isAvailable = m_data.m_isAvailable;
            _processingProgress = m_data.m_processingProgress;
            _rawLoot.Clear();
            _rawLoot.AddRange(m_data.m_rawLoot);
            _processedLoot.Clear();
            _processedLoot.AddRange(m_data.m_processedLoot);
            if (!_isAvailable)
            {
                StopProcessing();
            }
            else
            {
                for (int i = 1; i < m_data.m_buildingLevel; i++)
                {
                    Upgrade();
                }

                if(_processingProgress > 0 && _processingProgress < 100)
                {
                    ProcessLoot(true);
                }
            }
            Debug.Log(_buildingName + " Loaded");
        }
        else
        {
            Debug.LogWarning(_buildingName + " Load Error");
        }
    }

    public object Save()
    {
        MachineData m_data =  new MachineData()
        {
            m_buildingName = _buildingName,
            m_buildingLevel = _buildingLevel,
            m_buildingSpotName = _buildingSpot.gameObject.name,
            m_rawLoot = new Dictionary<string, int>(),
            m_processedLoot = new Dictionary<string, int>(),
            m_isAvailable = _isAvailable,
            m_processingProgress = _processingProgress,
        };
        m_data.m_rawLoot.AddRange(_rawLoot);
        m_data.m_processedLoot.AddRange(_processedLoot);
        return m_data;
    }
}

[System.Serializable]
public class MachineData
{
    public string m_buildingName;
    public int m_buildingLevel;
    public string m_buildingSpotName;
    public Dictionary<string, int> m_rawLoot;
    public Dictionary<string, int> m_processedLoot;
    public bool m_isAvailable;
    public int m_processingProgress;
}