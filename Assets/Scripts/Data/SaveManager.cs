using UnityEngine;
using System.IO;
using GameTools;


public struct PlayerData
{
    public int money;
    public int ammo;
    public int granades;
    public int health;
    

    public int checkpointsReached;
    public int npcsKilled;

    public bool hasGasCan;
    public bool hasMatches;
    public bool hasSunglasses;
    public PlayerData
        (
            int _money,
            int _ammo,
            int _grenades,
            int _health,
            int _checkpointsReached,
            int _npcsKilled,
            bool _hasGasCan,
            bool _hasMatches,
            bool _hasSunglasses

        )
    {
        money = _money;
        ammo = _ammo;
        granades = _grenades;
        health = _health;
        checkpointsReached = _checkpointsReached;
        npcsKilled = _npcsKilled;
        hasGasCan = _hasGasCan;
        hasMatches = _hasMatches;
        hasSunglasses = _hasSunglasses;
    }
}
public struct QuestStartedData
{
    public bool gasCan;
    public bool sunglasses;
    public bool matches;
    public bool final;

    public QuestStartedData
        (
            bool _gasCan,
            bool _sunglasses,
            bool _matches,
            bool _final
        )
    {
        gasCan = _gasCan;
        sunglasses = _sunglasses;
        matches = _matches;
        final = _final;
    }
}

/*
public struct QuestFinishedData
{
    public bool gasCan;
    public bool sunglasses;
    public bool matches;
    public bool final;

    public QuestFinishedData
        (
            bool _gasCan,
            bool _sunglasses,
            bool _matches,
            bool _final
        )
    {
        gasCan = _gasCan;
        sunglasses = _sunglasses;
        matches = _matches;
        final = _final;
    }
}
*/


public class SaveManager : MonoBehaviour
{

    private string SaveFilePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    /////////
    // API //
    /////////
    
    public bool SaveExists()
    {
        return File.Exists(SaveFilePath);
    }
    public void Save(PlayerData player, QuestStartedData questStarted)
    {
        SaveData data = ConvertToSaveData(player, questStarted);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(SaveFilePath, json);
    }

    public SaveData Load()
    {
        if (!File.Exists(SaveFilePath))
            return null;

        string json = File.ReadAllText(SaveFilePath);
        return JsonUtility.FromJson<SaveData>(json);

        
    }

    public void Clear()
    {
        if (File.Exists(SaveFilePath))
        {
            File.Delete(SaveFilePath);
        }
    }

    ////////////////////////
    // Conversion Helpers //
    ////////////////////////

    private SaveData ConvertToSaveData(PlayerData player, QuestStartedData questStarted)
    {
        return new SaveData
        {
            health = player.health,
            ammo = player.ammo,
            grenades = player.granades,
            money = player.money,
            npcsKilled = player.npcsKilled,
            checkpointsReached = player.checkpointsReached,
            hasGasCan = player.hasGasCan,
            hasMatches = player.hasMatches,
            hasSunglasses = player.hasSunglasses,

            gasCanQuestStarted = questStarted.gasCan,
            matchesQuestStarted = questStarted.matches,
            sunglassesQuestStarted = questStarted.sunglasses,
            finalQuestStarted = questStarted.final,
        };
    }
    public PlayerData ConvertPlayer(SaveData data)
    {
        return new PlayerData(
            data.money,
            data.ammo,
            data.grenades,
            data.health,
            data.checkpointsReached,
            data.npcsKilled,
            data.hasGasCan,
            data.hasMatches,
            data.hasSunglasses
        );
    }
    public QuestStartedData ConvertQuestStarted(SaveData data)
    {
        return new QuestStartedData(
            data.gasCanQuestStarted,
            data.sunglassesQuestStarted,
            data.matchesQuestStarted,
            data.finalQuestStarted
        );
    }

    /*
    public QuestFinishedData ConvertQuestFinished(SaveData data)
    {
        return new QuestFinishedData(
            data.gasCanQuestFinished,
            data.sunglassesQuestFinished,
            data.matchesQuestFinished,
            data.finalQuestFinished
        );
    }
    */

}
