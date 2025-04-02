using UnityEngine;
using UnityEngine.UI;

public class MinecartUIUpdater : MonoBehaviour
{
    [SerializeField] private Slider _capacitySlider;
    [SerializeField] private int _maxCapacity;

    public int MaxCapacity => _maxCapacity;

    private void Start()
    {
        _capacitySlider.maxValue = _maxCapacity;
    }

    public void UpdateCurrentCapacity(int capacity)
    {
        _capacitySlider.value = capacity;
    }
}
