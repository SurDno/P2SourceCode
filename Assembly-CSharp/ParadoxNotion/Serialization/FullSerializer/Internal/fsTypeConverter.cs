// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsTypeConverter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsTypeConverter : fsConverter
  {
    public override bool CanProcess(Type type) => typeof (Type).IsAssignableFrom(type);

    public override bool RequestCycleSupport(Type type) => false;

    public override bool RequestInheritanceSupport(Type type) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      Type type = (Type) instance;
      serialized = new fsData(type.FullName);
      return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      if (!data.IsString)
        return fsResult.Fail("Type converter requires a string");
      instance = (object) fsTypeCache.GetType(data.AsString);
      return instance == null ? fsResult.Fail("Unable to find type " + data.AsString) : fsResult.Success;
    }

    public override object CreateInstance(fsData data, Type storageType) => (object) storageType;
  }
}
