using UnityEngine;
using System.IO;
using GameTools;


public struct PlayerData
{
    public int money;
    //public int ammo;

    public int rifleTotalAmmo;
    public int pistolTotalAmmo;
    public int shotgunTotalAmmo;

    public int rifleInClipAmmo;
    public int pistolInClipAmmo;
    public int shotgunInClipAmmo;

    public int granades;
    public int health;

    // rifle ammo
    // pistol ammo
    // shotgun ammo

    // rifleAmmoCurrentClip
    // pistolAmmoCurrentClip
    // shotgunAmmoCurrentClip
    

    public int checkpointsReached;
    public int npcsKilled;

    public bool hasGasCan;
    public bool hasMatches;
    public bool hasSunglasses;
    public PlayerData
        (
            int _money,
            //int _ammo,
            int _rifleTotalAmmo,
            int _pistolTotalAmmo,
            int _shotgunTotalAmmo,
            int _rifleInClipAmmo,
            int _pistolInClipAmmo,
            int _shotgunInClipAmmo,
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
        //ammo = _ammo;
        rifleTotalAmmo = _rifleTotalAmmo;
        pistolTotalAmmo = _pistolTotalAmmo;
        shotgunTotalAmmo = _shotgunTotalAmmo;
        rifleInClipAmmo = _rifleInClipAmmo;
        pistolInClipAmmo = _pistolInClipAmmo;
        shotgunInClipAmmo = _shotgunInClipAmmo;
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

public struct SceneAndPlayerPos
{
    public Vector3 playerPos;
    public string sceneName;

    public SceneAndPlayerPos
        (
            Vector3 _playerPos,
            string _sceneName
        )
    {
        playerPos = _playerPos;
        sceneName = _sceneName;
    }
}

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
    public void Save(PlayerData player, QuestStartedData questStarted, SceneAndPlayerPos sceneData)
    {
        SaveData data = ConvertToSaveData(player, questStarted, sceneData);
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

    private SaveData ConvertToSaveData(PlayerData player, QuestStartedData questStarted, SceneAndPlayerPos sceneData)
    {
        return new SaveData
        {
            health = player.health,
            //ammo = player.ammo,

            rifleTotalAmmo = player.rifleTotalAmmo,
            pistolTotalAmmo = player.pistolTotalAmmo,
            shotgunTotalAmmo = player.shotgunTotalAmmo,

            rifleInClipAmmo = player.rifleInClipAmmo,
            pistolInClipAmmo = player.pistolInClipAmmo,
            shotgunInClipAmmo = player.shotgunInClipAmmo,

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

            playerPosX = sceneData.playerPos.x,
            playerPosY = sceneData.playerPos.y,
            playerposZ = sceneData.playerPos.z,
            gameSceneName = sceneData.sceneName

        };
    }
    public PlayerData ConvertPlayer(SaveData data)
    {
        return new PlayerData(
            data.money,
            //data.ammo,

            data.rifleTotalAmmo,
            data.pistolTotalAmmo,
            data.shotgunTotalAmmo,
            data.rifleInClipAmmo,
            data.pistolInClipAmmo,
            data.shotgunInClipAmmo,

            data.grenades,
            data.health,
            data.checkpointsReached,
            data.npcsKilled,
            data.hasGasCan,
            data.hasMatches,
            data.hasSunglasses
        );
    }

    public SceneAndPlayerPos ConvertSceneData(SaveData data)
    {
        Vector3 playerPos = new Vector3(data.playerPosX, data.playerPosY, data.playerposZ);
        string sceneName = data.gameSceneName;
        return new SceneAndPlayerPos(playerPos, sceneName);
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
}
