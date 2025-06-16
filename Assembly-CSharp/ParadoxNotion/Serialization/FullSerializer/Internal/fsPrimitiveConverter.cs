using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsPrimitiveConverter : fsConverter
  {
    public override bool CanProcess(Type type)
    {
      return type.Resolve().IsPrimitive || type == typeof (string) || type == typeof (Decimal);
    }

    public override bool RequestCycleSupport(Type storageType) => false;

    public override bool RequestInheritanceSupport(Type storageType) => false;

    private static bool UseBool(Type type) => type == typeof (bool);

    private static bool UseInt64(Type type)
    {
      return type == typeof (sbyte) || type == typeof (byte) || type == typeof (short) || type == typeof (ushort) || type == typeof (int) || type == typeof (uint) || type == typeof (long) || type == typeof (ulong);
    }

    private static bool UseDouble(Type type)
    {
      return type == typeof (float) || type == typeof (double) || type == typeof (Decimal);
    }

    private static bool UseString(Type type) => type == typeof (string) || type == typeof (char);

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      Type type = instance.GetType();
      if (this.Serializer.Config.Serialize64BitIntegerAsString && (type == typeof (long) || type == typeof (ulong)))
      {
        serialized = new fsData((string) Convert.ChangeType(instance, typeof (string)));
        return fsResult.Success;
      }
      if (fsPrimitiveConverter.UseBool(type))
      {
        serialized = new fsData((bool) instance);
        return fsResult.Success;
      }
      if (fsPrimitiveConverter.UseInt64(type))
      {
        serialized = new fsData((long) Convert.ChangeType(instance, typeof (long)));
        return fsResult.Success;
      }
      if (fsPrimitiveConverter.UseDouble(type))
      {
        if (instance.GetType() == typeof (float) && (double) (float) instance != -3.4028234663852886E+38 && (double) (float) instance != 3.4028234663852886E+38 && !float.IsInfinity((float) instance) && !float.IsNaN((float) instance))
        {
          serialized = new fsData((double) (Decimal) (float) instance);
          return fsResult.Success;
        }
        serialized = new fsData((double) Convert.ChangeType(instance, typeof (double)));
        return fsResult.Success;
      }
      if (fsPrimitiveConverter.UseString(type))
      {
        serialized = new fsData((string) Convert.ChangeType(instance, typeof (string)));
        return fsResult.Success;
      }
      serialized = (fsData) null;
      return fsResult.Fail("Unhandled primitive type " + (object) instance.GetType());
    }

    public override fsResult TryDeserialize(fsData storage, ref object instance, Type storageType)
    {
      fsResult success = fsResult.Success;
      if (fsPrimitiveConverter.UseBool(storageType))
      {
        fsResult fsResult;
        if ((fsResult = success + this.CheckType(storage, fsDataType.Boolean)).Succeeded)
          instance = (object) storage.AsBool;
        return fsResult;
      }
      if (fsPrimitiveConverter.UseDouble(storageType) || fsPrimitiveConverter.UseInt64(storageType))
      {
        if (storage.IsDouble)
          instance = !(storageType == typeof (float)) ? Convert.ChangeType((object) storage.AsDouble, storageType) : (object) (float) storage.AsDouble;
        else if (storage.IsInt64)
          instance = !(storageType == typeof (int)) ? Convert.ChangeType((object) storage.AsInt64, storageType) : (object) (int) storage.AsInt64;
        else if (this.Serializer.Config.Serialize64BitIntegerAsString && storage.IsString && (storageType == typeof (long) || storageType == typeof (ulong)))
          instance = Convert.ChangeType((object) storage.AsString, storageType);
        else
          return fsResult.Fail(this.GetType().Name + " expected number but got " + (object) storage.Type + " in " + (object) storage);
        return fsResult.Success;
      }
      if (!fsPrimitiveConverter.UseString(storageType))
        return fsResult.Fail(this.GetType().Name + ": Bad data; expected bool, number, string, but got " + (object) storage);
      fsResult fsResult1;
      if ((fsResult1 = success + this.CheckType(storage, fsDataType.String)).Succeeded)
        instance = (object) storage.AsString;
      return fsResult1;
    }
  }
}
