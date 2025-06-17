using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public class fsWeakReferenceConverter : fsConverter
  {
    public override bool CanProcess(Type type) => type == typeof (WeakReference);

    public override bool RequestCycleSupport(Type storageType) => false;

    public override bool RequestInheritanceSupport(Type storageType) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, Type storageType)
    {
      WeakReference weakReference = (WeakReference) instance;
      fsResult success = fsResult.Success;
      serialized = fsData.CreateDictionary();
      if (weakReference.IsAlive && !(success += Serializer.TrySerialize(weakReference.Target, out fsData data)).Failed)
      {
        serialized.AsDictionary["Target"] = data;
        serialized.AsDictionary["TrackResurrection"] = new fsData(weakReference.TrackResurrection);
      }
      return success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, Type storageType)
    {
      fsResult fsResult;
      if ((fsResult = fsResult.Success + CheckType(data, fsDataType.Object)).Failed || !data.AsDictionary.ContainsKey("Target"))
        return fsResult;
      fsData data1 = data.AsDictionary["Target"];
      object result = null;
      if ((fsResult += Serializer.TryDeserialize(data1, typeof (object), ref result)).Failed)
        return fsResult;
      bool trackResurrection = false;
      if (data.AsDictionary.ContainsKey("TrackResurrection") && data.AsDictionary["TrackResurrection"].IsBool)
        trackResurrection = data.AsDictionary["TrackResurrection"].AsBool;
      instance = new WeakReference(result, trackResurrection);
      return fsResult;
    }

    public override object CreateInstance(fsData data, Type storageType)
    {
      return new WeakReference(null);
    }
  }
}
