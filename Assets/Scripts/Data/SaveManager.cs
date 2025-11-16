using UnityEngine;
using System.IO;
using GameTools;


public struct PlayerData
{
    public int money;
    public int ammo;
    public int health;

    public bool hasGasCan;
    public bool hasMatches;
    public bool hasSunglasses;
    public PlayerData
        (
            int _money,
            int _ammo,
            int _health,
            bool _hasGasCan,
            bool _hasMatches,
            bool _hasSunglasses
        )
    {
        money = _money;
        ammo = _ammo;
        health = _health;
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


public class SaveManager : MonoBehaviour
{

    private string SaveFilePath =>
        Path.Combine(Application.persistentDataPath, "save.json");

    private void Awake()
    {
        // Lazy Singleton
        if (GameObject.FindGameObjectsWithTag(Constants.Tags.SAVE_MANAGER).Length > 0) { Destroy(gameObject); }
        tag = Constants.Tags.SAVE_MANAGER;
        DontDestroyOnLoad(gameObject);
    }

    /////////
    // API //
    /////////
    
    public bool SaveExists()
    {
        return File.Exists(SaveFilePath);
    }
    public void Save(PlayerData player, QuestStartedData quest)
    {
        SaveData data = ConvertToSaveData(player, quest);
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

    private SaveData ConvertToSaveData(PlayerData player, QuestStartedData quest)
    {
        return new SaveData
        {
            health = player.health,
            ammo = player.ammo,
            money = player.money,

            hasGasCan = player.hasGasCan,
            hasMatches = player.hasMatches,
            hasSunglasses = player.hasSunglasses,

            gasCanQuestStarted = quest.gasCan,
            matchesQuestStarted = quest.matches,
            sunglassesQuestStarted = quest.sunglasses,
            finalQuestStarted = quest.final
        };
    }
    public PlayerData ConvertPlayer(SaveData data)
    {
        return new PlayerData(
            data.money,
            data.ammo,
            data.health,
            data.hasGasCan,
            data.hasMatches,
            data.hasSunglasses
        );
    }
    public QuestStartedData ConvertQuest(SaveData data)
    {
        return new QuestStartedData(
            data.gasCanQuestStarted,
            data.sunglassesQuestStarted,
            data.matchesQuestStarted,
            data.finalQuestStarted
        );
    }

}
