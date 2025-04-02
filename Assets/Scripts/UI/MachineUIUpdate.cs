using UnityEngine;
using UnityEngine.UI;

public class MachineUIUpdate : MonoBehaviour
{
    [SerializeField] private string _name;
    [SerializeField] private TMPro.TextMeshProUGUI _levelText;
    [SerializeField] private Slider _progressSlider;

    public void UpdateProgress(int processingProgress)
    {
        _progressSlider.value = processingProgress;
    }

    public void UpdateLevel(int level)
    {
        _levelText.text = level.ToString();
    }

    public void Upgrade()
    {
        GameObject machine = GameObject.Find(_name);
        if (machine)
        {
            machine.GetComponent<Machine>().Upgrade();
        }
    }
}
