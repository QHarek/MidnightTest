using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.VisualScripting;

public class InventoryManager : MonoBehaviour, ISaveable
{
    [SerializeField] private StorageManager _storageExpander;
    [SerializeField] private InventoryUIUpdater _inventoryUIUpdater;
    [SerializeField] private CapacityUpdater _capacityUIUpdater;
    [SerializeField] private Transform _boxesParent;

    private List<string> _itemsToRecycle = new List<string>();
    private int _currentCapacity = 0;
    private bool _isFull = false;
    private bool _isreadyForRecycle = false;

    private Dictionary<string, int> _items = new Dictionary<string, int>()
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

    private Dictionary<string, int> _keepAmounts = new Dictionary<string, int>()
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

    public ReadOnlyDictionary<string, int> PublicItems;
    public int CurrentCapacity => _currentCapacity;
    public bool IsFull => _isFull;
    public bool IsReadyForRecycle => _isreadyForRecycle;

    private void Start()
    {
        SaveManager.Instance.inventoryManagerInstance = this;
        PublicItems = new ReadOnlyDictionary<string, int>(_items);
    }

    public void ImportItem(string itemId, int amount)
    {
        int importAmount = Mathf.Min(amount, _storageExpander.Capacity - _currentCapacity);
        if (_items.ContainsKey(itemId))
        {
            _items[itemId] += importAmount;
        }
        else
        {
            _items[itemId] = importAmount;
        }

        _currentCapacity += importAmount;
        _inventoryUIUpdater.UpdateInventoryUI(itemId, _items[itemId]);
        _capacityUIUpdater.UpdateCurrentCapacity(importAmount);
        EnableDisableBoxes(_currentCapacity);

        if (_currentCapacity == _storageExpander.Capacity)
        {
            _isFull = true;
        }
        if (GetAvailableItemAmount(itemId) > 0 && _itemsToRecycle.Contains(itemId))
        {
            _isreadyForRecycle = true;
        }
    }

    public int GetAvailableItemAmount(string itemId)
    {
        if (_items.ContainsKey(itemId))
        {
            return Mathf.Max(_items[itemId] - _keepAmounts[itemId], 0);
        }
        else
        {
            return 0;
        }
    }

    private void CheckRecycleAvailability()
    {
        foreach (var itemId in _itemsToRecycle)
        {
            if (_items[itemId] > 0)
            {
                _isreadyForRecycle = true;
                return;
            }
        }
        _isreadyForRecycle = false;
    }

    public bool CheckAvailableResources(List<string> resources)
    {
        foreach (var itemId in resources)
        {
            if (_itemsToRecycle.Contains(itemId) && GetAvailableItemAmount(itemId) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool IsAnyItemAvailable()
    {
        foreach(var itemId in _items)
        {
            if (GetAvailableItemAmount(itemId.Key) > 0)
            {
                return true;
            }
        }
        return false;
    }

    public void ExportItem(string itemId, int amount)
    {
        if (_items.ContainsKey(itemId))
        {
            _items[itemId] -= amount;
            _currentCapacity -= amount;
            _inventoryUIUpdater.UpdateInventoryUI(itemId, _items[itemId]);
            _capacityUIUpdater.UpdateCurrentCapacity(-amount);
            EnableDisableBoxes(_currentCapacity);

            if (_currentCapacity < _storageExpander.Capacity)
            {
                _isFull = false;
            }

            if (GetAvailableItemAmount(itemId) == 0 && _itemsToRecycle.Contains(itemId))
            {
                CheckRecycleAvailability();
            }
        }
    }

    public int ExportRecycleItem(string itemId, int amount)
    {
        if (_items.ContainsKey(itemId) && _itemsToRecycle.Contains(itemId))
        {
            _items[itemId] -= amount;
            _currentCapacity -= amount;
            _inventoryUIUpdater.UpdateInventoryUI(itemId, _items[itemId]);
            _capacityUIUpdater.UpdateCurrentCapacity(-amount);
            EnableDisableBoxes(_currentCapacity);

            if (_currentCapacity < _storageExpander.Capacity)
            {
                _isFull = false;
            }

            if (GetAvailableItemAmount(itemId) == 0 && _itemsToRecycle.Contains(itemId))
            {
                CheckRecycleAvailability();
            }

            return amount;
        }

        return 0;
    }

    public void MarkItemForRecycle(string itemId)
    {
        if (_itemsToRecycle.Contains(itemId))
        {
            _itemsToRecycle.Remove(itemId);
            if (_itemsToRecycle.Count == 0)
            {
                _isreadyForRecycle = false;
            }
        }
        else
        {
            _itemsToRecycle.Add(itemId);
            if (GetAvailableItemAmount(itemId) > 0)
            {
                _isreadyForRecycle = true;
            }
        }
    }

    public void KeepAmount(string itemId, int amount)
    {
        _keepAmounts[itemId] = amount;
    }

    private void EnableDisableBoxes(int capacity)
    {
        int boxesCount = capacity / 50;
        if (capacity % 50 > 0)
        {
            boxesCount++;
        }
        for (int i = 0; i < boxesCount; i++)
        {
            _boxesParent.GetChild(i).gameObject.SetActive(true);
        }
        for (int i = boxesCount; i < 80; i++)
        {
            _boxesParent.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void ExtendStorage()
    {
        _isFull = false;
    }

    public void Load(object data)
    {
        InventoryManagerData m_data = data as InventoryManagerData;
        if (m_data != null)
        {
            _currentCapacity = m_data.m_currentCapacity;
            _isFull = m_data.m_isFull;
            _isreadyForRecycle = m_data.m_readyForRecycle;
            _itemsToRecycle = new List<string>();
            _itemsToRecycle.AddRange(m_data.m_itemsToRecycle);
            _items = new Dictionary<string, int>();
            _items.AddRange(m_data.m_items);
            _keepAmounts = new Dictionary<string, int>();
            _keepAmounts.AddRange(m_data.m_keepAmounts);
            _inventoryUIUpdater.OnGameLoad(_keepAmounts, _itemsToRecycle);
            _capacityUIUpdater.UpdateCurrentCapacity(_currentCapacity);
            foreach (var item in _items)
            {
                _inventoryUIUpdater.UpdateInventoryUI(item.Key, item.Value);
            }
            EnableDisableBoxes(_currentCapacity);
            Debug.Log("InventoryData Loaded");
        }
        else
        {
            Debug.LogWarning("InventoryData Load Error");
        }
    }

    public object Save()
    {
        InventoryManagerData m_data = new InventoryManagerData()
        {
            m_currentCapacity = _currentCapacity,
            m_isFull = _isFull,
            m_readyForRecycle = _isreadyForRecycle,
            m_itemsToRecycle = new List<string>(),
            m_items = new Dictionary<string, int>(),
            m_keepAmounts = new Dictionary<string, int>(),
        };
        m_data.m_itemsToRecycle.AddRange(_itemsToRecycle);
        m_data.m_items.AddRange(_items);
        m_data.m_keepAmounts.AddRange(_keepAmounts);
        return m_data;
    }
}

[System.Serializable]
public class InventoryManagerData
{
    public int m_currentCapacity = 0;
    public bool m_isFull = false;
    public bool m_readyForRecycle = false;
    public List<string> m_itemsToRecycle;
    public Dictionary<string, int> m_items;
    public Dictionary<string, int> m_keepAmounts;
}