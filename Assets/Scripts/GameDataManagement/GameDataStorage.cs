using UnityEngine;
using System.IO;

namespace WaterSort
{
    public class GameDataStorage
    {
        string savePath;

        public GameDataStorage()
        {
            savePath = Path.Combine(Application.persistentDataPath, "data");
        }

        public void Save(IPersistentObject o)
        {
            using var writer = new BinaryWriter(File.Open(savePath, FileMode.OpenOrCreate));
            o.Save(new GameDataWriter(writer));
        }
        public void Load(IPersistentObject o)
        {
            if (File.Exists(savePath))
            {
                byte[] data = File.ReadAllBytes(savePath);
                using var reader = new BinaryReader(new MemoryStream(data));
                o.Load(new GameDataReader(reader));
            }
        }
    }
}