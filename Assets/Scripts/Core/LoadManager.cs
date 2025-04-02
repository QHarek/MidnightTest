using Newtonsoft.Json;
using System.IO;
using UnityEngine;

public class LoadManager : MonoBehaviour
{
    public static LoadManager Instance { get; private set; }
    public static GameData gameData;

    public string fileName = "gameData.json";
    private string FilePath => Path.Combine(Application.persistentDataPath, fileName);

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void LoadGame()
    {
        Time.timeScale = 1;

        if (!File.Exists(FilePath))
        {
            Debug.LogError("LoadManager: Файл сохранения не найден: " + FilePath);
            return;
        }

        string json = File.ReadAllText(FilePath);
        GameData gameData = JsonConvert.DeserializeObject<GameData>(json);
        Debug.Log("Игра загружена из " + FilePath);

        MineCart mineCart = FindObjectOfType<MineCart>();
        if (mineCart != null)
        {
            mineCart.Load(gameData.mineCartData);
        }
        else
        {
            Debug.LogWarning("MineCart не найден в сцене.");
        }

        Train train = FindObjectOfType<Train>();
        if (train != null)
        {
            train.Load(gameData.trainData);
        }

        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager != null)
        {
            settingsManager.Load(gameData.settingsManagerData);
        }

        ButtonPressedIndicator[] buttonIndicators = FindObjectsOfType<ButtonPressedIndicator>();
        if (gameData.buttonPressedIndicatorDataList != null)
        {
            int count = Mathf.Min(buttonIndicators.Length, gameData.buttonPressedIndicatorDataList.Count);
            for (int i = 0; i < count; i++)
            {
                buttonIndicators[i].Load(gameData.buttonPressedIndicatorDataList[i]);
            }
        }

        StorageManager storageManager = FindObjectOfType<StorageManager>();
        if (storageManager != null)
        {
            storageManager.Load(gameData.storageManagerData);
        }

        ShaftContent shaftContent = FindObjectOfType<ShaftContent>();
        if (shaftContent != null)
        {
            shaftContent.Load(gameData.shaftContentData);
        }

        ShaftMining shaftMining = FindObjectOfType<ShaftMining>();
        if (shaftMining != null)
        {
            shaftMining.Load(gameData.shaftMiningData);
        }

        InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.Load(gameData.inventoryManagerData);
        }

        WorkerHireManager workerHireManager = FindObjectOfType<WorkerHireManager>();
        if (workerHireManager != null)
        {
            workerHireManager.Load(gameData.workerHireManagerData);
        }

        BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
        if (buildingManager != null)
        {
            buildingManager.Load(gameData.buildingManagerData);
        }

        BalanceManager balanceManager = FindObjectOfType<BalanceManager>();
        if (balanceManager != null)
        {
            balanceManager.Load(gameData.balanceManagerData);
        }
    }
}
