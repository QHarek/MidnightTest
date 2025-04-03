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

        gameData.mineCartData = mineCartInstance.Save() as MineCartData;
        gameData.trainData = trainInstance.Save() as TrainData;

        if (settingsManagerInstance)
        {
            gameData.settingsManagerData = settingsManagerInstance.Save() as SettingsManagerData;
        }

        gameData.storageManagerData = storageManagerInstance.Save() as StorageManagerData;
        gameData.shaftContentData = shaftContentInstance.Save() as ShaftContentData;
        gameData.shaftMiningData = shaftMiningInstance.Save() as ShaftMiningData;
        gameData.inventoryManagerData = inventoryManagerInstance.Save() as InventoryManagerData;
        gameData.workerHireManagerData = workerHireManagerInstance.Save() as WorkerHireManagerData;
        gameData.buildingManagerData = buildingManagerInstance.Save() as BuildingManagerData;

        gameData.machineDataList = new List<MachineData>();
        for (int i = 0; machines.Count > i; i++)
        {
            if (machines[i] == null)
            {
                machines.RemoveAt(i);
                i--;
            }
        }
        foreach (Machine machine in machines)
        {
            gameData.machineDataList.Add(machine.Save() as MachineData);
        }

        gameData.balanceManagerData = balanceManagerInstance.Save() as BalanceManagerData;

        gameData.jobManagerDataList = new List<JobManagerData>();
        for (int i = 0; jobManagers.Count > i; i++)
        {
            if (jobManagers[i] == null)
            {
                jobManagers.RemoveAt(i);
                i--;
            }
        }
        foreach (JobManager job in jobManagers)
        {
            gameData.jobManagerDataList.Add(job.Save() as JobManagerData);
        }

        gameData.unloadCartJobDataList = new List<UnloadCartJobData>();
        for (int i = 0; unloadCartJobs.Count > i; i++)
        {
            if (unloadCartJobs[i] == null)
            {
                unloadCartJobs.RemoveAt(i);
                i--;
            }
        }
        foreach (UnloadCartJob job in unloadCartJobs)
        {
            gameData.unloadCartJobDataList.Add(job.Save() as UnloadCartJobData);
        }

        gameData.loadTrainJobDataList = new List<LoadTrainJobData>();
        for (int i = 0; loadTrainJobs.Count > i; i++)
        {
            if (loadTrainJobs[i] == null)
            {
                loadTrainJobs.RemoveAt(i);
                i--;
            }
        }
        foreach (LoadTrainJob job in loadTrainJobs)
        {
            gameData.loadTrainJobDataList.Add(job.Save() as LoadTrainJobData);
        }

        gameData.unloadMachineJobDataList = new List<UnloadMachineJobData>();
        for (int i = 0; unloadMachineJobs.Count > i; i++)
        {
            if (unloadMachineJobs[i] == null)
            {
                unloadMachineJobs.RemoveAt(i);
                i--;
            }
        }
        foreach (UnloadMachineJob job in unloadMachineJobs)
        {
            gameData.unloadMachineJobDataList.Add(job.Save() as UnloadMachineJobData);
        }

        gameData.loadMachineJobDataList = new List<LoadMachineJobData>();
        for (int i = 0; loadMachineJobs.Count > i; i++)
        {
            if (loadMachineJobs[i] == null)
            {
                loadMachineJobs.RemoveAt(i);
                i--;
            }
        }
        foreach (LoadMachineJob job in loadMachineJobs)
        {
            gameData.loadMachineJobDataList.Add(job.Save() as LoadMachineJobData);
        }

        string json = JsonConvert.SerializeObject(gameData, Formatting.Indented);
        File.WriteAllText(FilePath, json);
        Debug.Log("Игра сохранена в " + FilePath);
    }
}
