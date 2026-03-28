using System.Collections.Generic;
using System.Linq;

namespace SSMPEssentials.Data
{
    internal class PlayerDataTracker
    {
        internal static PlayerDataTracker ClientInstance = new();
        internal static PlayerDataTracker ServerInstance = new();

        readonly Dictionary<ushort, StoredPlayerData> data = new();

        public StoredPlayerData GetPlayer(ushort id)
        {
            if (data.TryGetValue(id, out StoredPlayerData playerData))
            {
                return playerData;
            }

            playerData = new StoredPlayerData(id);

            data.Add(id, playerData);
            return playerData;
        }

        public List<StoredPlayerData> GetAllData()
        {
            return data.Values.ToList();
        }
    }

    internal class StoredPlayerData
    {
        public ushort Id;
        public HealthData Health = new();

        public StoredPlayerData(ushort id)
        {
            Id = id;
        }
    }

    public class HealthData
    {
        public int Health = 5;
        public int MaxHealth = 5;
        public int BlueHealth = 0;
        public bool LifebloodState = false;

        public override string ToString()
        {
            return $"{Health}/{MaxHealth} + {BlueHealth} ({LifebloodState})";
        }
    }
}
