using L4RH;
using L4RH.Model.Textures;
using L4RH.Readers;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TexturePackHeaderChunks : IChunkReader
{
    public uint ChunkId => 0xB3310000;

    public void Deserialize(BinarySpan span, TexturePack pack)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;

        var header = new TexturePackHeaderReader();
        var index = new TextureIndexEntryReader();
        var info = new TextureInfoReader();
        var dynamic = new TextureStreamEntryReader();

        while (span.Pointer < span.Length)
        {
            span.SkipPadding();

            uint id = span.ReadUInt32();
            int length = span.ReadInt32();

            span.Pointer -= 8;

            var chunkBuffer = new BinarySpan(span.ReadArray(length + 8));

            if (id == header.ChunkId)
            {
                header.Deserialize(chunkBuffer, pack);
            }
            else if (id == index.ChunkId)
            {
                index.Deserialize(chunkBuffer, pack);
            }
            else if (id == info.ChunkId)
            {
                info.Deserialize(chunkBuffer, pack);
            }
            else if (id == dynamic.ChunkId)
            {
                dynamic.Deserialize(chunkBuffer, pack);
            }
        }
    }
}
