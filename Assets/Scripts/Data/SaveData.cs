using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int health;
    public int ammo;
    public int money;

    public bool hasSunglasses;
    public bool hasMatches;
    public bool hasGasCan;

    public bool sunglassesQuestStarted;
    public bool matchesQuestStarted;
    public bool gasCanQuestStarted;
    public bool finalQuestStarted;
}
