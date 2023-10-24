using L4RH;
using L4RH.Model.Solids;
using L4RH.Readers;
using System;
using System.Numerics;

namespace UG2Mappings.SolidListReaders;

internal class SolidObjectHeader : IChunkReader
{
    public uint ChunkId => 0x00134011;

    public SolidObject Deserialize(BinarySpan span, SolidObjectList list)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        span.Pointer += 4;

        span.AlignPosition();

        span.Pointer += 12;

        SolidObject obj = new()
        {
            Version = span.ReadByte()
        };

        span.Pointer += 1;

        obj.Flags = (SolidFlags)span.ReadUInt16();
        obj.Key = span.ReadUInt32();
        obj.IndicesNumber = span.ReadUInt16();

        span.Pointer += 10;

        obj.MinPoint = span.ReadStruct<Vector3>();
        span.Pointer += 4;
        obj.MaxPoint = span.ReadStruct<Vector3>();
        span.Pointer += 4;
        obj.Matrix = span.ReadStruct<Matrix4x4>();

        span.Pointer += 20;

        obj.Volume = span.ReadSingle() / 1000000000;
        obj.Density = span.ReadSingle();

        span.Pointer += 8;

        obj.Name = span.ReadString(0x1C);

        list.Add(obj);

        return obj;
    }
}
