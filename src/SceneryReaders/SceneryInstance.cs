using L4RH;
using L4RH.Model.Sceneries;
using L4RH.Readers;
using System;
using System.Diagnostics;
using System.Numerics;

using SceneryInstanceClass = L4RH.Model.Sceneries.SceneryInstance;

namespace UG2Mappings.SceneryReaders;

internal class SceneryInstance : IChunkReader
{
    public uint ChunkId => 0x00034103;

    public void Deserialize(BinarySpan span, Scenery scenery)
    {
        if (span.ReadUInt32() != ChunkId)
            throw new ArgumentException("Unsupported chunk!");

        var length = span.ReadInt32();
        var start = span.Pointer;
        span.AlignPosition();
        var chunkStart = span.Pointer - start;

        Debug.Assert((length - chunkStart) % 0x40 == 0);

        var size = (length - chunkStart) / 0x40;

        for (var i = 0; i < size; i++)
        {
            var instance = new SceneryInstanceClass(scenery)
            {
                BoundBoxMin = span.ReadStruct<Vector3>(),
                BoundBoxMax = span.ReadStruct<Vector3>(),
                SceneryInfo = span.ReadUInt16(),
                Flags = (InstanceFlags)span.ReadUInt16(),
                PreCullerInfo = span.ReadInt32()
            };

            var pos = span.ReadStruct<Vector3>();
            var v1 = new Vector3(span.ReadInt16(), span.ReadInt16(), span.ReadInt16()) / 8192;
            var v2 = new Vector3(span.ReadInt16(), span.ReadInt16(), span.ReadInt16()) / 8192;
            var v3 = new Vector3(span.ReadInt16(), span.ReadInt16(), span.ReadInt16()) / 8192;

            span.Pointer += 2;

            instance.InstanceMatrix = new Matrix4x4(
                v1.X, v1.Y, v1.Z, 0,
                v2.X, v2.Y, v2.Z, 0,
                v3.X, v3.Y, v3.Z, 0,
                pos.X, pos.Y, pos.Z, 1) * Matrix4x4.Identity with { M11 = -1 };

            scenery.ObjectInstances.Add(instance);
        }
    }
}
