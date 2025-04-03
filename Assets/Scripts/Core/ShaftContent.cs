using System.Collections.Generic;
using UnityEngine;

public enum Crystals
{
    CrystalCommon,
    CrystalUncommon,
    CrystalRare,
    CrystalEpic
}

public class ShaftContent : MonoBehaviour, ISaveable
{
    [SerializeField] private ShaftUIUpdater _shaftUIUpdater;
    [SerializeField] private List<GameObject> _outerCrystals;
    [SerializeField] private List<Crystals> _lootTypes;
    [SerializeField] private int _upgradeCost;

    private int _level = 1;
    private int _maxLevel = 4;

    private void Start()
    {
        SaveManager.Instance.shaftContentInstance = this;
    }

    private void UpgradeOuterCrystals()
    {
        Color color = Color.grey;
        foreach (var crystal in _outerCrystals)
        {
            switch (Random.Range(0, _level))
            {
                case 0:
                    {
                        color = Color.grey;
                        break;
                    }
                case 1: 
                    {
                        color = Color.green;
                        break;
                    }
                case 2:
                    {
                        color = Color.blue;
                        break;
                    }
                case 3:
                    {
                        color = Color.magenta;
                        break;
                    }
            }

            crystal.GetComponent<OuterCrystalChanger>().UpdateColor(color);
        }
    }

    public void UpgradeShaft()
    {
        if(_level < _maxLevel)
        {
            _lootTypes.Add((Crystals)_level);
            FindObjectOfType<BalanceManager>().AddRegularExpediture(200, "Rent");
            FindObjectOfType<BalanceManager>().RemoveMoney(_upgradeCost, null);
            _level++;
            _shaftUIUpdater.UpdateShaftUILevel(_level);
            UpgradeOuterCrystals();
        }
    }

    internal string GetRandomCrystal()
    {
        return _lootTypes[Random.Range(0, _lootTypes.Count)].ToString();
    }

    public void Load(object data)
    {
        ShaftContentData m_data = data as ShaftContentData;
        if (m_data != null)
        {
            for (int i = 1; i < m_data.Level; i++)
            {
                UpgradeShaft();
            }
            Debug.Log("ShaftContent Loaded");
        }
        else
        {
            Debug.LogWarning("ShaftContent Load Error");
        }
    }

    public object Save()
    {
        return new ShaftContentData()
        {
            Level = _level,
        };
    }
}

[System.Serializable]
public class ShaftContentData
{
    public int Level;
}
