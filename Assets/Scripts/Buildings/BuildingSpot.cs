using System.Collections;
using UnityEngine;

public class BuildingSpot : MonoBehaviour, IBuildingManagerObserver
{
    private bool _isOccupied = false;
    private Machine _building;
    private Renderer _renderer;
    private Color _defaultColor;
    private BuildingManager _buildingManager;

    public bool IsOccupied => _isOccupied;
    public Renderer Renderer => _renderer;
    public Color DefaultColor => _defaultColor;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _defaultColor = _renderer.material.color;
        _buildingManager = FindObjectOfType<BuildingManager>();
    }

    public void OnBuildingAction(BuildingAction action, Machine building)
    {
        if (action == BuildingAction.Build && building.BuildingSpot == this)
        {
            _isOccupied = true;
            _building = building;
        }
        else if (action == BuildingAction.Destroy && building == _building)
        {
            _building = null;
            StartCoroutine(FreeSlot());
        }
    }

    public void OnBuildingSpotOccupied()
    {
        _isOccupied = true;
    }

    private void OnMouseDown()
    {
        if (_buildingManager != null && _buildingManager.enabled)
        {
            if (_buildingManager.DestroyMode && _building != null)
            {
                _buildingManager.DestroyBuilding(_building);
            }
            else
            {
                _buildingManager.BuildBuilding(this);
            }
        }
    }

    private IEnumerator FreeSlot()
    {            
        yield return new WaitForSeconds(15);
        _isOccupied = false;
    }
}