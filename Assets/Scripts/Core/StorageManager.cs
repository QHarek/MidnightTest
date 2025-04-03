using UnityEngine;

public class StorageManager : MonoBehaviour, ISaveable
{
    [SerializeField] private int _capacity = 1000;
    [SerializeField] private CapacityUpdater _capacityUpdater;

    private int _activeUnitIndex = 1;

    public int Capacity => _capacity;

    private void Start()
    {
        SaveManager.Instance.storageManagerInstance = this;
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
            ActiveUnitIndex = _activeUnitIndex,
        };
    }

    public void Load(object data)
    {
        StorageManagerData m_data = data as StorageManagerData;
        if (m_data != null)
        {
            for (int i = 1; i < m_data.ActiveUnitIndex; i++)
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
    public int ActiveUnitIndex;
}