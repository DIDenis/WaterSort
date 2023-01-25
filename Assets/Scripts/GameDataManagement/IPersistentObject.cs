namespace WaterSort
{
    public interface IPersistentObject
    {
        void Save(GameDataWriter writer);
        void Load(GameDataReader reader);
    }
}