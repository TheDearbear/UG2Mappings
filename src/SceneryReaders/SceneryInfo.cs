using L4RH.Readers;
using L4RH;
using SceneryInfoClass = L4RH.Model.Sceneries.SceneryInfo;
using L4RH.Model.Sceneries;
using System.Diagnostics;
using System;

namespace UG2Mappings.SceneryReaders;

internal class SceneryInfo : IChunkReader
{
    public uint ChunkId => 0x00034102;

    public void Deserialize(BinarySpan span, Scenery scenery)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        var length = span.ReadInt32() / 0x44;

        Debug.Assert(span.Length == length * 0x44 + 8);

        for (var i = 0; i < length; i++)
        {
            var info = new SceneryInfoClass()
            {
                Name = span.ReadString(0x20),
                SolidLodA = span.ReadUInt32(),
                SolidLodB = span.ReadUInt32(),
                SolidLodC = span.ReadUInt32(),
                SolidLodAFlags = span.ReadUInt16(),
                SolidLodBFlags = span.ReadUInt16()
            };

            span.Pointer += 12;

            info.Radius = span.ReadSingle();
            info.HierarchyKey = span.ReadUInt32();

            scenery.ObjectInfos.Add(info);
        }
    }
}
