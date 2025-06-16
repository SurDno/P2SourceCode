// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.fsDirectConverter`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
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
