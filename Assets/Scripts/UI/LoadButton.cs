using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadButton : MonoBehaviour
{
    [SerializeField] private string sceneToLoad = "GameScene";

    public void OnLoadButtonPressed()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene(sceneToLoad);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == sceneToLoad)
        {
            if (LoadManager.Instance != null)
            {
                LoadManager.Instance.LoadGame();
            }
            else
            {
                Debug.LogWarning("LoadManager instance не найден!");
            }

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
}
