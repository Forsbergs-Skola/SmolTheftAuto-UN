using System;

namespace SmolTheftAuto.Core
{
    // Interface for systems that can save and load their state
    // Used by the save/load system to persist game state
    public interface ISaveable
    {
        // Get the unique identifier for this saveable object
        string GetSaveID();

        // Save current state to a serializable format
        SaveData SaveState();

        // Load state from saved data
        void LoadState(SaveData data);
    }

    // Base class for save data
    [Serializable]
    public class SaveData
    {
        public string saveID;
        public string dataType;
    }
}

