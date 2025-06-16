using System.Collections.Generic;
using UnityEngine;

namespace ParadoxNotion.Serialization.FullSerializer.Internal.DirectConverters
{
  public class LayerMask_DirectConverter : fsDirectConverter<LayerMask>
  {
    protected override fsResult DoSerialize(LayerMask model, Dictionary<string, fsData> serialized)
    {
      return fsResult.Success + this.SerializeMember<int>(serialized, (System.Type) null, "value", model.value);
    }

    protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref LayerMask model)
    {
      fsResult success = fsResult.Success;
      int num = model.value;
      fsResult fsResult = success + this.DeserializeMember<int>(data, (System.Type) null, "value", out num);
      model.value = num;
      return fsResult;
    }

    public override object CreateInstance(fsData data, System.Type storageType)
    {
      return (object) new LayerMask();
    }
  }
}
