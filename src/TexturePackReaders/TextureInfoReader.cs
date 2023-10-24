using L4RH;
using L4RH.Model.Textures;
using L4RH.Readers;
using System;

namespace UG2Mappings.TexturePackReaders;

internal class TextureInfoReader : IChunkReader
{
    public uint ChunkId => 0x33310004;

    public void Deserialize(BinarySpan span, TexturePack pack)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        int entries = span.ReadInt32() / 0x7C;

        for (int i = 0; i < entries; i++)
        {
            TextureInfo texture = Deserialize(ref span);

            pack.InfoEntries[i] = texture;

            foreach (var index in pack.IndexEntries)
                if (index.Hash == texture.NameHash)
                    index.Texture = texture;
        }
    }

    public static TextureInfo Deserialize(ref BinarySpan span)
    {
        // Left data is less than 0x7C bytes
        if (span.Length - span.Pointer < 0x7C)
            throw new ArgumentException("Data is too small!", nameof(span));

        span.Pointer += 12;

        TextureInfo texture = new()
        {
            Name = span.ReadString(0x18),
            NameHash = span.ReadUInt32(),
            ClassHash = span.ReadUInt32(),
            ParentImageHash = span.ReadUInt32(),
            DataOffset = span.ReadInt32(),
            PaletteOffset = span.ReadInt32(),
            DataSize = span.ReadInt32(),
            PaletteSize = span.ReadInt32(),
            BaseImageSize = span.ReadInt32(),
            Width = span.ReadInt16(),
            Height = span.ReadInt16(),
            WidthShift = span.ReadByte(),
            HeightShift = span.ReadByte(),
            DataCompression = span.ReadByte(),
            PaletteCompression = span.ReadByte(),
            PaletteCount = span.ReadInt16(),
            MipmapCount = span.ReadByte(),
            TileableUV = span.ReadByte(),
            BiasLevel = span.ReadByte(),
            RenderOrder = span.ReadByte(),
            ScrollType = span.ReadByte(),
            FlagsUsed = span.ReadByte() != 0,
            ApplyAlphaSort = span.ReadByte() != 0,
            AlphaUsageType = span.ReadByte(),
            AlphaBlendType = span.ReadByte(),
            Flags = span.ReadByte(),
            ScrollTimestep = span.ReadInt16(),
            ScrollSpeedS = span.ReadInt16(),
            ScrollSpeedT = span.ReadInt16(),
            OffsetS = span.ReadInt16(),
            OffsetT = span.ReadInt16(),
            ScaleS = span.ReadInt16(),
            ScaleT = span.ReadInt16()
        };

        span.Pointer += 0x16;

        return texture;
    }
}
