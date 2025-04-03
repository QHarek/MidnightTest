using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class InventoryUIUpdater : MonoBehaviour
{
    [SerializeField] private List<InventorySlot> _inventorySlots;

    public void UpdateInventoryUI(string itemId, int amount)
    {
        _inventorySlots.Where(m => m.name.Contains(itemId)).First().UpdateSlot(amount);
    }

    public void OnGameLoad(Dictionary<string, int> keepAmounts, List<string> recycleList)
    {
        foreach (var slot in _inventorySlots)
        {
            var itemKeep = keepAmounts.Where(m => slot.name.Contains(m.Key)).FirstOrDefault();
            var itemRecycle = recycleList.Where(m => slot.name.Contains(m)).FirstOrDefault();
            slot.OnGameLoad(itemKeep, itemRecycle);
        }
    }
}