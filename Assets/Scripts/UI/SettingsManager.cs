using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour, ISaveable
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private Slider _volumeSlider;

    private void Start()
    {
        SaveManager.Instance.settingsManagerInstance = this;
    }

    public void SwitchMusic()
    {
        _musicSource.enabled = !_musicSource.enabled;
    }

    public void UpdateVolume()
    {
        _musicSource.volume = _volumeSlider.value;
    }

    public void Load(object data)
    {
        SettingsManagerData m_data = data as SettingsManagerData;
        if (m_data != null)
        {
            _musicSource.enabled = m_data.m_isMusicEnabled;
            _musicSource.volume = m_data.m_volume;
            Debug.Log("Settings Loaded");
        }
        else
        {
            Debug.LogWarning("Settings Load Error");
        }
    }

    public object Save()
    {
        return new SettingsManagerData()
        {
            m_volume = _volumeSlider.value,
            m_isMusicEnabled = _musicSource.enabled,
        };
    }
}

[System.Serializable]
public class SettingsManagerData
{
    public float m_volume;
    public bool m_isMusicEnabled;
}
