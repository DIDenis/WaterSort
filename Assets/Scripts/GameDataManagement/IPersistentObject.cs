using System.IO;

public interface IPersistentObject
{
    void Save(GameDataWriter writer);
    void Load(GameDataReader reader);
}
