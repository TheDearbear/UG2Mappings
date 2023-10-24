using L4RH.Model.Textures;
using L4RH.Readers;
using L4RH;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TextureStreamEntryReader : IChunkReader
{
    public uint ChunkId => 0x33310003;

    public void Deserialize(BinarySpan span, TexturePack pack)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        int count = span.ReadInt32() / 0x18;
        pack.StreamEntries = new TextureStreamEntry[count];

        for (int i = 0; i < count; i++)
        {
            pack.StreamEntries[i] = new TextureStreamEntry
            {
                Hash = span.ReadUInt32(),
                DataOffset = span.ReadInt32(),
                CompressedDataSize = span.ReadInt32(),
                DecompressedDataSize = span.ReadInt32()
            };

            span.Pointer += 8;
        }
    }
}
