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
}