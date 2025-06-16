using System;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization.FullSerializer
{
  public abstract class fsDirectConverter<TModel> : fsDirectConverter
  {
    public override Type ModelType => typeof (TModel);

    public override sealed fsResult TrySerialize(
      object instance,
      out fsData serialized,
      Type storageType)
    {
      Dictionary<string, fsData> dictionary = new Dictionary<string, fsData>();
      fsResult fsResult = this.DoSerialize((TModel) instance, dictionary);
      serialized = new fsData(dictionary);
      return fsResult;
    }

    public override sealed fsResult TryDeserialize(
      fsData data,
      ref object instance,
      Type storageType)
    {
      fsResult fsResult1;
      if ((fsResult1 = fsResult.Success + this.CheckType(data, fsDataType.Object)).Failed)
        return fsResult1;
      TModel model = (TModel) instance;
      fsResult fsResult2 = fsResult1 + this.DoDeserialize(data.AsDictionary, ref model);
      instance = (object) model;
      return fsResult2;
    }

    protected abstract fsResult DoSerialize(TModel model, Dictionary<string, fsData> serialized);

    protected abstract fsResult DoDeserialize(Dictionary<string, fsData> data, ref TModel model);
  }
}
