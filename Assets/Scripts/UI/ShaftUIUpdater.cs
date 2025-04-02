using UnityEngine;

public class ShaftUIUpdater : MonoBehaviour
{
    [SerializeField] TMPro.TextMeshProUGUI _levelText;
    [SerializeField] TMPro.TextMeshProUGUI _workersText;
    [SerializeField] Transform _lootParent;

    public void UpdateShaftUILevel(int level)
    {
        _levelText.text = level.ToString();

        _lootParent.GetChild(level).gameObject.SetActive(true);
    }

    public void UpdateShaftUIWorkers(int workersCount)
    {
        _workersText.text = workersCount.ToString();
    }
}
