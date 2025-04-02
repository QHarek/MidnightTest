using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance { get; private set; }

    public MineCart mineCartInstance;
    public Train trainInstance;
    public SettingsManager settingsManagerInstance;
    public List<ButtonPressedIndicator> buttonPressedIndicatorsInstances;
    public StorageManager storageManagerInstance;
    public ShaftContent shaftContentInstance;
    public ShaftMining shaftMiningInstance;
    public InventoryManager inventoryManagerInstance;
    public WorkerHireManager workerHireManagerInstance;
    public BuildingManager buildingManagerInstance;
    public List<Machine> machines;
    public BalanceManager balanceManagerInstance;
    public List<JobManager> jobManagers;
    public List<UnloadCartJob> unloadCartJobs;
    public List<LoadTrainJob> loadTrainJobs;
    public List<UnloadMachineJob> unloadMachineJobs;
    public List<LoadMachineJob> loadMachineJobs;

    private string fileName = "gameData.json";
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

    public void SaveGame()
    {
        GameData gameData = new GameData();

        gameData.mineCartData = FindObjectOfType<MineCart>().Save() as MineCartData;
        gameData.trainData = FindObjectOfType<Train>().Save() as TrainData;

        SettingsManager settingsManager = FindObjectOfType<SettingsManager>();
        if (settingsManager)
        {
            gameData.settingsManagerData = settingsManager.Save() as SettingsManagerData;
        }

        buttonPressedIndicatorsInstances.AddRange(FindObjectsOfType<ButtonPressedIndicator>());
        gameData.buttonPressedIndicatorDataList = new List<ButtonPressedIndicatorData>();
        foreach (ButtonPressedIndicator indicator in buttonPressedIndicatorsInstances)
        {
            gameData.buttonPressedIndicatorDataList.Add(indicator.Save() as ButtonPressedIndicatorData);
        }
        //

        gameData.storageManagerData = FindObjectOfType<StorageManager>().Save() as StorageManagerData;
        gameData.shaftContentData = FindObjectOfType<ShaftContent>().Save() as ShaftContentData;
        gameData.shaftMiningData = FindObjectOfType<ShaftMining>().Save() as ShaftMiningData;
        gameData.inventoryManagerData = FindObjectOfType<InventoryManager>().Save() as InventoryManagerData;
        gameData.workerHireManagerData = FindObjectOfType<WorkerHireManager>().Save() as WorkerHireManagerData;
        gameData.buildingManagerData = FindObjectOfType<BuildingManager>().Save() as BuildingManagerData;

        machines.AddRange(FindObjectsOfType<Machine>());
        gameData.machineDataList = new List<MachineData>();
        foreach (Machine machine in machines)
        {
            gameData.machineDataList.Add(machine.Save() as MachineData);
        }

        gameData.balanceManagerData = FindObjectOfType<BalanceManager>().Save() as BalanceManagerData;


        jobManagers.AddRange(FindObjectsOfType<JobManager>());
        gameData.jobManagerDataList = new List<JobManagerData>();
        foreach (JobManager job in jobManagers)
        {
            gameData.jobManagerDataList.Add(job.Save() as JobManagerData);
        }

        unloadCartJobs.AddRange(FindObjectsOfType<UnloadCartJob>());
        gameData.unloadCartJobDataList = new List<UnloadCartJobData>();
        foreach (UnloadCartJob job in unloadCartJobs)
        {
            gameData.unloadCartJobDataList.Add(job.Save() as UnloadCartJobData);
        }

        loadTrainJobs.AddRange(FindObjectsOfType<LoadTrainJob>());
        gameData.loadTrainJobDataList = new List<LoadTrainJobData>();
        foreach (LoadTrainJob job in loadTrainJobs)
        {
            gameData.loadTrainJobDataList.Add(job.Save() as LoadTrainJobData);
        }

        unloadMachineJobs.AddRange(FindObjectsOfType<UnloadMachineJob>());
        gameData.unloadMachineJobDataList = new List<UnloadMachineJobData>();
        foreach (UnloadMachineJob job in unloadMachineJobs)
        {
            gameData.unloadMachineJobDataList.Add(job.Save() as UnloadMachineJobData);
        }

        loadMachineJobs.AddRange(FindObjectsOfType<LoadMachineJob>());
        gameData.loadMachineJobDataList = new List<LoadMachineJobData>();
        foreach (LoadMachineJob job in loadMachineJobs)
        {
            gameData.loadMachineJobDataList.Add(job.Save() as LoadMachineJobData);
        }

        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(FilePath, json);
        Debug.Log("Игра сохранена в " + FilePath);
    }
}
