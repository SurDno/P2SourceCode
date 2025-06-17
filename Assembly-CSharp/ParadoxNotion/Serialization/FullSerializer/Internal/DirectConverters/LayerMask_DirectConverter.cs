using System;
using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
  {
    protected override fsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + SerializeMember(serialized, null, "value", model.value);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
    {
      fsResult fsResult = fsResult.Success;
      fsResult += DeserializeMember(data, null, "value", out int num);
      model.value = num;
      return fsResult;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new LayerMask();
    }
  }
}
