using L4RH;
using L4RH.Model.Textures;
using L4RH.Readers;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TextureVRAMDataChunks : IChunkReader
{
    public uint ChunkId => 0xB3320000;

    public void Deserialize(BinarySpan span, TexturePack pack, long totalRead)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;

        var raw = new TexturePackDataRaw();

        while (span.Pointer < span.Length)
        {
            span.SkipPadding();

            uint id = span.ReadUInt32();
            int length = span.ReadInt32();

            span.Pointer -= 8;

            var chunkBuffer = new BinarySpan(span.ReadArray(length + 8));

            if (id == raw.ChunkId)
            {
                raw.Deserialize(chunkBuffer, pack, totalRead + span.Pointer - chunkBuffer.Length);
            }
        }
    }
}
