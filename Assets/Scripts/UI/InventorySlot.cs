using UnityEngine;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _itemCountText;
    [SerializeField] private TMP_InputField _amountInputField;
    [SerializeField] private InventoryManager _inventoryManager;
    [SerializeField] private string _itemId;

    public string ItemId => _itemId;
    private bool _keepThis = false;

    public void UpdateSlot(int itemCount)
    {
        _itemCountText.text = itemCount.ToString();
    }

    public void OnRecycleButtonClicked()
    {
        _inventoryManager.MarkItemForRecycle(_itemId);
    }

    public void OnKeepAmountButtonClicked()
    {
        _keepThis = !_keepThis;
        UpdateKeepAmount();
    }

    private void UpdateKeepAmount()
    {
        if (_amountInputField.text.Length > 0)
        {
            int newAmount = int.Parse(_amountInputField.text);
            _inventoryManager.KeepAmount(_itemId, newAmount);
        }
    }

    public void OnCountUpdate()
    {
        if (_keepThis)
        {
            UpdateKeepAmount();
        }
    }
}