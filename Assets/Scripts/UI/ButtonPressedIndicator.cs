using UnityEngine;
using UnityEngine.UI;

public class ButtonPressedIndicator : MonoBehaviour
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

    public void OnGameLoad(bool isEnabled)
    {
        if (isEnabled)
        {
            _image.color = Color.green;
            _toggled = true;
        }
    }
}
