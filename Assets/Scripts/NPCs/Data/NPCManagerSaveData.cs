using SmolTheftAuto.Core;
using UnityEngine;

namespace SmolTheftAuto.NPCs.Managers
{
    // Save data structure for NPCManager state
    // Used for save/load functionality
    [System.Serializable]
    public class NPCManagerSaveData : SaveData
    {
        public string saveID;
    public string dataType;
        public int activeNPCCount;
        public int maxNPCCount;
        public float spawnRadius;
        public bool useSpawnCenter;
        public Vector3 spawnCenter;
    }
}

