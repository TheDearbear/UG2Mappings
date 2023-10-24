using L4RH;
using L4RH.Model.Textures;
using L4RH.Readers;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TexturePackHeaderReader : IChunkReader
{
    public uint ChunkId => 0x33310001;

    public void Deserialize(BinarySpan span, TexturePack pack)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;

        pack.Version = (TexturePackVersion)span.ReadInt32();
        pack.Name = span.ReadString(0x1C);
        pack.PipelinePath = span.ReadString(0x40);

        span.Pointer += 28;
    }
}
