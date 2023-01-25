using System.IO;

namespace WaterSort
{
    public class GameDataWriter
    {
        BinaryWriter writer;

        public GameDataWriter(BinaryWriter writer)
        {
            this.writer = writer;
        }

        public void Write(int value)
        {
            writer.Write(value);
        }
        public void Write(bool value)
        {
            writer.Write(value);
        }
    }
}