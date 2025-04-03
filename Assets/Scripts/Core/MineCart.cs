using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MineCart : MonoBehaviour, ISaveable
{
    [SerializeField] private GameObject _visibleLoot;
    [SerializeField] private MinecartUIUpdater _mineCartUIUpdater;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _pathTime;

    private Dictionary<string, int> _loot = new Dictionary<string, int>();

    private int _currentCapacity;
    private int _maxCapacity;

    private bool _isAvailableToFill = true;
    private bool _isAvailableToUnload = false;

    public int CurrentCapacity => _currentCapacity;
    public bool IsAvailableToUnload => _isAvailableToUnload;

    private void Start()
    {
        SaveManager.Instance.mineCartInstance = this;
        _maxCapacity = _mineCartUIUpdater.MaxCapacity;
    }

    private IEnumerator MoveToWarehouse()
    {
        _visibleLoot.SetActive(true);
        _isAvailableToFill = false;
        _animator.Play("CartToWarehouse");
        yield return new WaitForSeconds(_pathTime);
        _isAvailableToUnload = true;
    }

    private IEnumerator MoveToShaft()
    {
        _visibleLoot.SetActive(false);
        _isAvailableToUnload = false;
        _animator.Play("CartToMine");

        _loot.Clear();
        _currentCapacity = 0;
        _mineCartUIUpdater.UpdateCurrentCapacity(_currentCapacity);

        yield return new WaitForSeconds(_pathTime);
        _isAvailableToFill = true;
    }

    public void AddLoot(string crystalName, int crystalCount)
    {
        if (_isAvailableToFill)
        {
            int amount = crystalCount;
            if (crystalCount > _maxCapacity - _currentCapacity)
            {
                amount = _maxCapacity - _currentCapacity;
            }

            if (_loot.ContainsKey(crystalName))
            {
                _loot[crystalName] += amount;
            }
            else
            {
                _loot.Add(crystalName, amount);
            }

            _currentCapacity += amount;
            _mineCartUIUpdater.UpdateCurrentCapacity(_currentCapacity);

            if (_currentCapacity == _maxCapacity)
            {
                StartCoroutine(MoveToWarehouse());
            }
        }
    }

    public Dictionary<string, int> UnloadLoot()
    {
        Dictionary<string, int> loot = new Dictionary<string, int>();
        loot.AddRange(_loot);
        StartCoroutine(MoveToShaft());
        return loot;
    }

    public void Load(object data)
    {
        StopAllCoroutines();
        MineCartData m_data = data as MineCartData;
        if (m_data != null)
        {
            _loot.Clear();
            _loot.AddRange(m_data.m_loot);
            _currentCapacity = m_data.m_currentCapacity;
            _isAvailableToFill = m_data.m_isAvailableToFill;
            _isAvailableToUnload = m_data.m_isAvailableToUnload;
            _maxCapacity = _mineCartUIUpdater.MaxCapacity;
            transform.position = m_data.m_position.ToVector3();

            if (_isAvailableToFill && _currentCapacity == 0)
            {
                StartCoroutine(MoveToShaft());
            }
            if (_isAvailableToUnload && _currentCapacity == _maxCapacity)
            {
                StartCoroutine(MoveToWarehouse());
            }
            Debug.Log("Minecart Loaded");
        }
        else
        {
            Debug.LogWarning("Minecart Load Error");
        }
    }

    public object Save()
    {
        MineCartData m_data = new MineCartData()
        {
            m_loot = new Dictionary<string, int>(),
            m_position = new SerializableVector3(transform.position),
            m_currentCapacity = _currentCapacity,
            m_isAvailableToFill = _isAvailableToFill,
            m_isAvailableToUnload = _isAvailableToUnload,
        };
        m_data.m_loot.AddRange(_loot);
        return m_data;
    }
}

[System.Serializable]
public class MineCartData
{
    public Dictionary<string, int> m_loot;
    public SerializableVector3 m_position;
    public int m_currentCapacity;
    public bool m_isAvailableToUnload;
    public bool m_isAvailableToFill;
}
