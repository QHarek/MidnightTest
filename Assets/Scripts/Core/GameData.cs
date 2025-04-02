using System;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public MineCartData mineCartData;
    public TrainData trainData;
    public SettingsManagerData settingsManagerData;
    public List<ButtonPressedIndicatorData> buttonPressedIndicatorDataList;
    public StorageManagerData storageManagerData;
    public ShaftContentData shaftContentData;
    public ShaftMiningData shaftMiningData;
    public InventoryManagerData inventoryManagerData;
    public WorkerHireManagerData workerHireManagerData;
    public BuildingManagerData buildingManagerData;
    public List<MachineData> machineDataList;
    public BalanceManagerData balanceManagerData;
    public List<JobManagerData> jobManagerDataList;
    public List<UnloadCartJobData> unloadCartJobDataList;
    public List<LoadTrainJobData> loadTrainJobDataList;
    public List<UnloadMachineJobData> unloadMachineJobDataList;
    public List<LoadMachineJobData> loadMachineJobDataList;
}
