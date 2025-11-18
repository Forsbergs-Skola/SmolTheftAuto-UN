namespace SmolTheftAuto.Core
{
    // Interface for NPC spawning systems to decouple NPCHealth from NPCSpawner
    public interface INPCSpawner
    {
        void RespawnNPC(UnityEngine.GameObject npc, float delay);
    }
}

