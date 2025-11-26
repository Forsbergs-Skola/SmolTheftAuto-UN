using UnityEngine;
using System.Collections.Generic;
using Events;
using GameTools;

public class GameplaySceneMaganer : MonoBehaviour
{
    
    [Header("Spawn Positions -- Drag empty GameObjects from the scene hierarchy")]
    [Tooltip("A vehicle will spawn at every transform in this list")]
    [SerializeField] private List<Transform> vehicleSpawnPoints;
    [Tooltip("An NPC will spawn at every transform in this list")]
    [SerializeField] private List<Transform> npcSpawnPoints;
    [Tooltip("The player will spawn at a randomly chosen transform from this list")]
    [SerializeField] private List<Transform> playerSpawnPoints;

    [Header("Prefabs -- Drag these from the project window")]
    [Tooltip("Drag from Assets/Prefabs/Player")]
    [SerializeField] private GameObject playerPrefab;
    [Tooltip("One for each unique vehicle prefab. Drag them from Assets/Prefabs/Vehicles")]
    [SerializeField] private List<GameObject> vehiclePrefabs;
    [Tooltip("One for each unique NPC prefab. Drag them from Assets/Prefabs/NPCs")]
    [SerializeField] private List<GameObject> npcPrefabs;

    private void Start()
    {
        SpawnPlayer();
        SpawnNPCs();
        SpawnVehicles();
    }

    private void SpawnPlayer()
    {
        // TODO
    }
    private void SpawnNPCs()
    {
        // TODO
    }
    private void SpawnVehicles()
    {
        // TODO
    }

}
