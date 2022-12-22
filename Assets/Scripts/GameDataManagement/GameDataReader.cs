using UnityEngine;
using System.IO;

public class GameDataReader
{
    BinaryReader reader;

    public GameDataReader(BinaryReader reader)
    {
        this.reader = reader;
    }

    public int ReadInt()
    {
        return reader.ReadInt32();
    }
    public bool ReadBool()
    {
        return reader.ReadBoolean();
    }
}
