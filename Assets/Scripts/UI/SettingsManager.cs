using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour, ISaveable
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private Slider _volumeSlider;

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
            _musicSource.enabled = m_data.IsMusicEnabled;
            _musicSource.volume = m_data.Volume;
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
            Volume = _volumeSlider.value,
            IsMusicEnabled = _musicSource.enabled,
        };
    }
}

[System.Serializable]
public class SettingsManagerData
{
    public float Volume;
    public bool IsMusicEnabled;
}
