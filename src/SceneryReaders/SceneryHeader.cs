using L4RH;
using L4RH.Model.Sceneries;
using L4RH.Readers;
using System;
using System.Diagnostics;

namespace UG2Mappings.SceneryReaders;

internal class SceneryHeader : IChunkReader
{
    public uint ChunkId => 0x00034101;

    public void Deserialize(BinarySpan span, Scenery scenery)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        Debug.Assert(span.Length == 68);

        span.Pointer += 16;

        scenery.VisibleSectionId = span.ReadInt32();

        span.Pointer += 44;
    }
}
