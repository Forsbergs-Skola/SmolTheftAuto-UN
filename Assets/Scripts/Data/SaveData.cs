using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int health;
    public int ammo;
    public int grenades;
    public int money;

    public int checkpointsReached;
    public int npcsKilled;

    public bool hasSunglasses;
    public bool hasMatches;
    public bool hasGasCan;

    public bool sunglassesQuestStarted;
    public bool matchesQuestStarted;
    public bool gasCanQuestStarted;
    public bool finalQuestStarted;

    public bool sunglassesQuestFinished;
    public bool matchesQuestFinished;
    public bool gasCanQuestFinished;
    public bool finalQuestFinished;

    public float playerPosX;
    public float playerPosY;
    public float playerposZ;
    public string gameSceneName;

}
