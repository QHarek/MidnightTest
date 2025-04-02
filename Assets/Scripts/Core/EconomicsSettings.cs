using UnityEngine;

[CreateAssetMenu(fileName = "New Economics Data", menuName = "Economics Data", order = 0)]
public class EconomicsSettings : ScriptableObject
{
    [Header("General Values")]
    public int rentCost;

    [Header("Crystal Values")]
    public int commonCrystalValue;
    public int uncommonCrystalValue;
    public int rareCrystalValue;
    public int epicCrystalValue;

    [Header("Dust Values")]
    public int commonDustValue;
    public int uncommonDustValue;
    public int rareDustValue;
    public int epicDustValue;
}