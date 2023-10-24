using L4RH;
using L4RH.Compression;
using L4RH.Model.Textures;
using L4RH.Readers;
using System;
using System.Diagnostics;

namespace UG2Mappings.TexturePackReaders;

internal class TexturePackDataRaw : IChunkReader
{
    public uint ChunkId => 0x33320002;

    public void Deserialize(BinarySpan span, TexturePack pack, long totalRead)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;
        span.AlignPosition();

        int dataBegin = span.Pointer;

        foreach (var entry in pack.StreamEntries)
        {
            span.Pointer = (int)(entry.DataOffset - totalRead);

            var data = span.ReadArray(entry.CompressedDataSize).ToArray();

            try
            {
                data = LZ.Decompress(data);
            }
            catch (ArgumentException e)
            {
                Debug.WriteLine($"Got error while decompressing {nameof(TextureStreamEntry)} with hash 0x{entry.Hash:X8}: " + e);
                continue;
            }

            // 0x9C bytes: TextureInfo + 2 bytes align + TextureInfoPlatInfo
            var metadata = new byte[0x9C];
            Array.Copy(data, data.Length - 0x9C, metadata, 0, 0x9C);
            var metadataSpan = new BinarySpan(metadata);

            TextureInfo info = TextureInfoReader.Deserialize(ref metadataSpan);
            info.Data = data;

            if (info.PaletteSize > 0)
            {
                span.Pointer = dataBegin + info.PaletteOffset;
                info.Palette = span.ReadArray(info.PaletteSize).ToArray();
            }

            UpdateIndexTexture(pack, entry.Hash, info);
        }

        foreach (var texture in pack.InfoEntries)
        {
            if (texture is null)
            {
                Debug.WriteLine($"Empty {nameof(TextureInfo)} in {nameof(TexturePack)} '{pack.Name}'");
                continue;
            }

            if (texture.DataSize > 0 && texture.Data.Length > 0)
                continue;

            if (texture.PaletteSize > 0)
            {
                span.Pointer = dataBegin + texture.PaletteOffset;
                texture.Palette = span.ReadArray(texture.PaletteSize).ToArray();
            }

            int dataPos = dataBegin + texture.DataOffset;
            span.Pointer = dataPos;//- (dataPos & 0xFF);
            texture.Data = span.ReadArray(texture.DataSize).ToArray();
        }
    }

    static void UpdateIndexTexture(TexturePack pack, uint hash, TextureInfo? info)
    {
        foreach (var index in pack.IndexEntries)
            if (index.Hash == hash)
                index.Texture = info;
    }
}
