using UnityEngine;
using UnityEngine.UI;

public class ButtonPressedIndicator : MonoBehaviour, ISaveable
{
    [SerializeField] private Image _image;
    private Color _initialColor;
    private bool _toggled;

    private void Start()
    {
        _image = GetComponent<Image>();
        _initialColor = _image.color;
        _toggled = false;
    }

    public void OnButtonClick()
    {
        if (_toggled)
        {
            _image.color = _initialColor;
            _toggled = false;
        }
        else
        {
            _image.color = Color.green;
            _toggled = true;
        }
    }

    public void Load(object data)
    {
        ButtonPressedIndicatorData m_data = data as ButtonPressedIndicatorData;
        if (m_data != null)
        {
            _image.color = new Color(m_data.ColorR, m_data.ColorG, m_data.ColorB, m_data.ColorA);
            _toggled = m_data.IsToggled;
            Debug.Log("Button" + gameObject.name + " Loaded");
        }
        else
        {
            Debug.LogWarning("Button" + gameObject.name + " Load Error");
        }
    }

    public object Save()
    {
        return new ButtonPressedIndicatorData()
        {
            ColorR = _image.color.r,
            ColorG = _image.color.g,
            ColorB = _image.color.b,
            ColorA = _image.color.a,
            IsToggled = _toggled,
        };
    }
}

[System.Serializable]
public class ButtonPressedIndicatorData
{
    public float ColorR;
    public float ColorG;
    public float ColorB;
    public float ColorA;
    public bool IsToggled;
}
