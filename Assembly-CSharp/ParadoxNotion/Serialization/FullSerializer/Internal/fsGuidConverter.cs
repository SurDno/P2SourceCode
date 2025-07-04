﻿using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsGuidConverter : fsConverter
  {
    public override bool CanProcess(Type type) => type == typeof (Guid);

    public override bool RequestCycleSupport(Type storageType) => false;

    public override bool RequestInheritanceSupport(Type storageType) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      Guid guid = (Guid) instance;
      serialized = new fsData(guid.ToString());
      return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      if (!data.IsString)
        return fsResult.Fail("fsGuidConverter encountered an unknown JSON data type");
      instance = new Guid(data.AsString);
      return fsResult.Success;
    }

    public override object CreateInstance(fsData data, Type storageType) => new Guid();
  }
}
