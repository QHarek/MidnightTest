using System.Collections.Generic;
using UnityEngine;

public class BalanceUIUpdate : MonoBehaviour
{
    [SerializeField] private TMPro.TextMeshProUGUI _balanceText;
    [SerializeField] private TMPro.TextMeshProUGUI _incomeText;
    [SerializeField] private TMPro.TextMeshProUGUI _expeditureText;
    [SerializeField] private TMPro.TextMeshProUGUI _rentText;
    [SerializeField] private TMPro.TextMeshProUGUI _minersText;
    [SerializeField] private TMPro.TextMeshProUGUI _workersText;
    [SerializeField] private TMPro.TextMeshProUGUI _machinesText;
    [SerializeField] private TMPro.TextMeshProUGUI _crystalCommonText;
    [SerializeField] private TMPro.TextMeshProUGUI _crystalUncommonText;
    [SerializeField] private TMPro.TextMeshProUGUI _crystalRareText;
    [SerializeField] private TMPro.TextMeshProUGUI _crystalEpicText;
    [SerializeField] private TMPro.TextMeshProUGUI _dustCommonText;
    [SerializeField] private TMPro.TextMeshProUGUI _dustUncommonText;
    [SerializeField] private TMPro.TextMeshProUGUI _dustRareText;
    [SerializeField] private TMPro.TextMeshProUGUI _dustEpicText;
    [SerializeField] private BalanceManager _balanceManager;

    private void Start()
    {
        _balanceManager = GetComponent<BalanceManager>();
    }

    public void UpdateBalanceUI(string type, Dictionary<string, int> list)
    {
        Transform parent = transform;
        if(type == "income")
        {
            int income = 0;
            foreach (KeyValuePair<string, int> pair in list)
            {
                income += pair.Value;
            }

            _crystalCommonText.text = list["CrystalCommon"].ToString();
            _crystalUncommonText.text = list["CrystalUncommon"].ToString();
            _crystalRareText.text = list["CrystalRare"].ToString();
            _crystalEpicText.text = list["CrystalEpic"].ToString();
            _dustCommonText.text = list["DustCommon"].ToString();
            _dustUncommonText.text = list["DustUncommon"].ToString();
            _dustRareText.text = list["DustRare"].ToString();
            _dustEpicText.text = list["DustEpic"].ToString();

            _incomeText.text = income.ToString();
            _balanceText.text = _balanceManager.Balance.ToString();
        }
        else
        {
            int expediture = 0;
            foreach (KeyValuePair<string, int> pair in list)
            {
                expediture -= pair.Value;
            }

            _rentText.text = list["Rent"].ToString();
            _minersText.text = list["Miners"].ToString();
            _workersText.text = list["Workers"].ToString();
            _machinesText.text = list["Machines"].ToString();

            _expeditureText.text = (Mathf.Abs(expediture)).ToString();
            _balanceText.text = _balanceManager.Balance.ToString();
        }

        _balanceManager.ClearLists();
    }
}
