using System;
using UnityEngine;
using System.Collections.Generic;

[Serializable]
public class GameData
{
    public MineCartData mineCartData;
    public TrainData trainData;
    public SettingsManagerData settingsManagerData;
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

[Serializable]
public class SerializableVector3
{
    public float x, y, z;

    public SerializableVector3(Vector3 vector)
    {
        x = vector.x;
        y = vector.y;
        z = vector.z;
    }

    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[Serializable]
public class SerializableColor
{
    public float r, g, b, a;

    public SerializableColor(Color color)
    {
        r = color.r;
        g = color.g;
        b = color.b;
        a = color.a;
    }

    public Color ToColor()
    {
        return new Color(r, g, b, a);
    }
}

[Serializable]
public class SerializableQuaternion
{
    public float x, y, z, w;

    public SerializableQuaternion(Quaternion quaternion)
    {
        x = quaternion.x;
        y = quaternion.y;
        z = quaternion.z;
        w = quaternion.w;
    }

    public Quaternion ToQuaternion()
    {
        return new Quaternion(x, y, z, w);
    }
}
