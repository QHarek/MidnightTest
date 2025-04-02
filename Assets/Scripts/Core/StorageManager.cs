using UnityEngine;

public class StorageManager : MonoBehaviour, ISaveable
{
    [SerializeField] private int _capacity = 1000;
    [SerializeField] private CapacityUpdater _capacityUpdater;

    private int _activeUnitIndex = 1;

    public int Capacity => _capacity;

    private void Start()
    {
        _capacityUpdater.UpdateMaxCapacity(_capacity);
    }

    public void ExpandStorage()
    {
        while (_activeUnitIndex < transform.childCount && transform.GetChild(_activeUnitIndex).gameObject.activeSelf)
        {
            _activeUnitIndex++;
        }

        if (_activeUnitIndex < transform.childCount)
        {
            Transform child = transform.GetChild(_activeUnitIndex);
            child.gameObject.SetActive(true);
            FindObjectOfType<BalanceManager>().AddRegularExpediture(100, "Rent");
            FindObjectOfType<BalanceManager>().RemoveMoney(2000, null);
            _capacity += 1000;
            _capacityUpdater.UpdateMaxCapacity(_capacity);
            _activeUnitIndex++;
        }
    }

    public object Save()
    {
        return new StorageManagerData()
        {
            Capacity = _capacity,
            ActiveUnitIndex = _activeUnitIndex,
        };
    }

    public void Load(object data)
    {
        StorageManagerData m_data = data as StorageManagerData;
        if (m_data != null)
        {
            _capacity = m_data.Capacity;
            _activeUnitIndex = m_data.ActiveUnitIndex;
            for (int i = 0; i < m_data.ActiveUnitIndex; i++)
            {
                ExpandStorage();
            }
            Debug.Log("StorageManager Loaded");
        }
        else
        {
            Debug.LogWarning("StorageManager Load Error");
        }
    }
}

[System.Serializable]
public class StorageManagerData
{
    public int Capacity;
    public int ActiveUnitIndex;
}