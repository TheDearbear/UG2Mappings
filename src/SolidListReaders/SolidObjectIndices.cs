using L4RH;
using L4RH.Model.Solids;
using L4RH.Readers;
using System;
using System.Diagnostics;

namespace UG2Mappings.SolidListReaders;

internal class SolidObjectIndices : IChunkReader
{
    public uint ChunkId => 0x00134B03;

    const int VERTEX_PER_INDEX = 3;

    public void Deserialize(BinarySpan span, SolidObject solid)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        int length = span.ReadInt32();
        length -= span.AlignPosition();

        // I don't understand fricking format (length == 0x66C, but solid.IndicesNumber == 283)
        // Debug.Assert(length / VERTEX_PER_INDEX / sizeof(ushort) == solid.IndicesNumber, "Every index set contains 3 shorts");

        solid.Indices = new ushort[length / sizeof(ushort)];

        for (int i = 0; i < solid.Indices.Length; i++)
            solid.Indices[i] = span.ReadUInt16();
    }
}
