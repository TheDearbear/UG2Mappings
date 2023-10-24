using L4RH;
using L4RH.Model.Sceneries;
using L4RH.Readers;
using System;

namespace UG2Mappings.SceneryReaders;

public class SceneryReader : ISceneryReader
{
    public uint ChunkId => 0x80034100;

    public object DeserializeObject(BinarySpan span, long dataBasePosition) => Deserialize(span, dataBasePosition);
    public Scenery Deserialize(BinarySpan span, long dataBasePosition)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        int spanLength = span.ReadInt32() + 8;

        var scenery = new Scenery() { Offset = (uint)dataBasePosition };

        var header = new SceneryHeader();
        var info = new SceneryInfo();
        var instance = new SceneryInstance();

        while (span.Pointer < spanLength)
        {
            span.SkipPadding();

            var id = span.ReadUInt32();
            var length = span.ReadInt32();

            span.Pointer -= 8;

            var chunkBuffer = new BinarySpan(span.ReadArray(length + 8));

            if (id == header.ChunkId)
            {
                header.Deserialize(chunkBuffer, scenery);
            }
            else if (id == info.ChunkId)
            {
                info.Deserialize(chunkBuffer, scenery);
            }
            else if (id == instance.ChunkId)
            {
                instance.Deserialize(chunkBuffer, scenery);
            }
        }

        return scenery;
    }
}
