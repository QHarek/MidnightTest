using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CapacityUpdater : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _maxCapacityText;
    [SerializeField] private TextMeshProUGUI _currentCapacityText;
    [SerializeField] private Slider _capacitySlider;

    private int _currentCapacity;
    private int _maxCapacity;

    public void UpdateMaxCapacity(int capacity)
    {
        _maxCapacityText.text = capacity.ToString();
        _maxCapacity = capacity;
        UpdateSlider();
    }

    public void UpdateCurrentCapacity(int capacity)
    {
        _currentCapacity += capacity;
        _currentCapacityText.text = _currentCapacity.ToString();
        FindObjectOfType<InventoryManager>().ExtendStorage();
        UpdateSlider();
    }

    private void UpdateSlider()
    {
        _capacitySlider.maxValue = _maxCapacity;
        _capacitySlider.value = _currentCapacity;
    }
}