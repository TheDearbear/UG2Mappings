using L4RH;
using L4RH.Model;
using L4RH.Readers;
using System;

namespace UG2Mappings.CollisionVolumeReaders;

internal class VolumeHeader : IChunkReader
{
    public uint ChunkId => 0x00039200;

    public CollisionVolume Deserialize(BinarySpan span)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;
        span.AlignPosition();
        span.Pointer += 8;

        var volume = new CollisionVolume()
        {
            SomeHash = span.ReadUInt32(),
        };

        span.Pointer += 8;

        volume.Vertices = new float[span.ReadUInt32() * 3];
        span.Pointer += 4;
        volume.Name = span.ReadString(0x20);
        volume.Indices = new ushort[span.ReadUInt16()];

        span.Pointer += 26;

        return volume;
    }
}
