using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _gui;
    [SerializeField] GameObject _settings;

    private bool _isGamePaused = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isGamePaused)
            {
                Resume();
            }
            else
            { 
                Pause();
            }
        }
    }

    public void Pause()
    {
        _gui.SetActive(false);
        ShowHidePause(true);
        Time.timeScale = 0;
        _isGamePaused = true;
    }

    public void Resume()
    {
        _gui.SetActive(true);
        ShowHidePause(false);
        Time.timeScale = 1;
        _isGamePaused = false;
    }

    public void GoToMainMenu()
    {
        SaveGame();
        SceneManager.LoadScene("MainMenu");
    }

    public void SaveGame()
    {
        SaveManager.Instance.SaveGame();
    }

    public void ShowSettings()
    {
        _settings.SetActive(true);
    }

    private void ShowHidePause(bool isShowing)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject child = transform.GetChild(i).gameObject;
            if (!(child.name == "Settings" && isShowing))
            {
                transform.GetChild(i).gameObject.SetActive(isShowing);
            }
        }
    }
}
