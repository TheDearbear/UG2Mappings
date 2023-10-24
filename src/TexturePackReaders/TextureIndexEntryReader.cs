using L4RH.Model.Textures;
using L4RH;
using L4RH.Readers;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TextureIndexEntryReader : IChunkReader
{
    public uint ChunkId => 0x33310002;

    public void Deserialize(BinarySpan span, TexturePack pack)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        int count = span.ReadInt32() / 8;

        pack.IndexEntries = new TextureIndexEntry[count];
        pack.InfoEntries = new TextureInfo[count];

        for (int i = 0; i < count; i++)
        {
            pack.IndexEntries[i] = new TextureIndexEntry
            {
                Hash = span.ReadUInt32()
            };

            span.Pointer += 4;
        }
    }
}
