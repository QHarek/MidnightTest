using UnityEngine;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;

public class BuildingManager : MonoBehaviour, ISaveable
{
    [SerializeField] private Transform _buildingSpotsParent;
    [SerializeField] private List<GameObject> _buildingPrefabs ;
    [SerializeField] private int _maxBuildingsPerType = 1;

    private Dictionary<string, int> _buildingCounts = new Dictionary<string, int>();
    private List<IBuildingManagerObserver> _observers = new List<IBuildingManagerObserver>();
    private List<Machine> _builtMachines = new List<Machine>();
    private Machine _currentBuilding;
    private bool _buildingMode = false;
    private bool _destroyMode = false;

    public bool DestroyMode => _destroyMode;
    public List<Machine> BuiltMachines => _builtMachines;
    public ReadOnlyDictionary<string, int> BuildingCounts;

    private void Awake()
    {
        _observers = new List<IBuildingManagerObserver>();
        foreach (Transform child in _buildingSpotsParent)
        {
            BuildingSpot spot = child.GetComponent<BuildingSpot>();
            if (spot != null)
            {
                _observers.Add(spot);
            }
        }
        BuildingCounts = new ReadOnlyDictionary<string, int>(_buildingCounts);
    }

    private void Start()
    {
        SaveManager.Instance.buildingManagerInstance = this;
    }

    private void NotifyObservers(BuildingAction action, Machine building)
    {
        foreach (IBuildingManagerObserver observer in _observers)
        {
            observer.OnBuildingAction(action, building);
        }
    }

    public void AddObserver(IBuildingManagerObserver observer)
    {
        _observers.Add(observer);
    }

    public void RemoveObserver(IBuildingManagerObserver observer)
    {
        _observers.Remove(observer);
    }

    public void SelectBuilding(Machine buildingPrefab)
    {
        if (_currentBuilding == buildingPrefab)
        {
            _currentBuilding = null;
            _buildingMode = false;
        }
        else
        {
            _currentBuilding = buildingPrefab;
            _buildingMode = true;
            _destroyMode = false;
        }
        UpdateBuildingSpotsColor();
    }

    public void BuildBuilding(BuildingSpot spot)
    {
        if (_buildingMode && _currentBuilding != null && !spot.IsOccupied)
        {
            string buildingName = _currentBuilding.BuildingName;
            if (!_buildingCounts.ContainsKey(buildingName))
            {
                _buildingCounts[buildingName] = 0;
            }

            if (_buildingCounts[buildingName] < _maxBuildingsPerType)
            {
                spot.OnBuildingSpotOccupied();
                Machine building = Instantiate(_currentBuilding, spot.transform.position + new Vector3(0, 0.3f, 0), Quaternion.identity);
                building.name = buildingName;
                building.SetBuildingSpot(spot);
                _builtMachines.Add(building);
                NotifyObservers(BuildingAction.Build, building);
                _buildingCounts[buildingName]++;
                _currentBuilding = null;
                _buildingMode = false;
                UpdateBuildingSpotsColor();
            }
            else
            {
                Debug.Log("ƒостигнуто максимальное количество машин данного типа.");
            }
        }
        else
        {
            Debug.Log("Ќевозможно построить здесь.");
        }
    }

    public void DestroyBuilding(Machine buildingToDestroy)
    {
        if (_destroyMode && buildingToDestroy != null)
        {
            NotifyObservers(BuildingAction.Destroy, buildingToDestroy);
            UpdateBuildingSpotsColor();
        }
    }

    public void OnSpotCleared(Machine buildingToDestroy)
    {
        _builtMachines.Remove(buildingToDestroy);
        string buildingName = buildingToDestroy.BuildingName;
        if (_buildingCounts.ContainsKey(buildingName))
        {
            _buildingCounts[buildingName]--;
        }
    }

    public void ToggleDestroyMode()
    {
        _destroyMode = !_destroyMode;
        _buildingMode = false;
        _currentBuilding = null;
        UpdateBuildingSpotsColor();
    }

    private void UpdateBuildingSpotsColor()
    {
        for (int i = 0; i < _buildingSpotsParent.childCount; i++)
        {
            Transform child = _buildingSpotsParent.GetChild(i);
            BuildingSpot spot = child.GetComponent<BuildingSpot>();
            if (spot != null && spot.Renderer != null)
            {
                if (_buildingMode && !spot.IsOccupied)
                {
                    spot.Renderer.material.color = Color.green;
                }
                else if (_destroyMode && spot.IsOccupied)
                {
                    spot.Renderer.material.color = Color.red;
                }
                else
                {
                    spot.Renderer.material.color = spot.DefaultColor;
                }
            }
        }
    }

    private IEnumerator LoadComponents(int index)
    {
        Machine machineInstance = null;
        yield return new WaitUntil(() => _builtMachines[index].TryGetComponent(out machineInstance));
        machineInstance.Load(LoadManager.gameData.machineDataList[index]);
    }

    public void Load(object data)
    {
        BuildingManagerData m_data = data as BuildingManagerData;
        if (m_data != null)
        {
            _builtMachines.Clear();
            _buildingCounts.Clear();
            for (int i = 0; i < m_data.m_builtMachines.Count; i++)
            {
                _buildingMode = true;
                GameObject prefab = _buildingPrefabs.Where(m => m.name == m_data.m_builtMachines.ElementAt(i).Key).First();
                _currentBuilding = prefab.GetComponent<Machine>();
                GameObject spot = GameObject.Find(m_data.m_builtMachines.ElementAt(i).Value);
                BuildBuilding(spot.GetComponent<BuildingSpot>());
                StartCoroutine(LoadComponents(i));
            }
            Debug.Log("BuildingManagerData Loaded");
        }
        else
        {
            Debug.LogWarning("BuildingManagerData Load Error");
        }
    }

    public object Save()
    {
        BuildingManagerData data = new BuildingManagerData()
        {
            m_builtMachines = new Dictionary<string, string>(),
        };
        foreach (var item in _builtMachines)
        {
            data.m_builtMachines.Add(item.name, item.BuildingSpot.name);
        }

        return data;
    }
}

[System.Serializable]
public class BuildingManagerData
{
    public Dictionary<string, string> m_builtMachines;
}