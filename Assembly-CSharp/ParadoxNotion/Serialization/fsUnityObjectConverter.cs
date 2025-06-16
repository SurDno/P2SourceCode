using ParadoxNotion.Serialization.FullSerializer;
using System.Collections.Generic;

namespace ParadoxNotion.Serialization
{
  public class fsUnityObjectConverter : fsConverter
  {
    public override bool CanProcess(System.Type type) => typeof (UnityEngine.Object).RTIsAssignableFrom(type);

    public override bool RequestCycleSupport(System.Type storageType) => false;

    public override bool RequestInheritanceSupport(System.Type storageType) => false;

    public override fsResult TrySerialize(object instance, out fsData serialized, System.Type storageType)
    {
      List<UnityEngine.Object> objectList = this.Serializer.Context.Get<List<UnityEngine.Object>>();
      UnityEngine.Object @object = instance as UnityEngine.Object;
      if ((object) @object == null)
      {
        serialized = new fsData(0L);
        return fsResult.Success;
      }
      if (objectList.Count == 0)
        objectList.Add((UnityEngine.Object) null);
      int i = -1;
      for (int index = 0; index < objectList.Count; ++index)
      {
        if ((object) objectList[index] == (object) @object)
        {
          i = index;
          break;
        }
      }
      if (i <= 0)
      {
        i = objectList.Count;
        objectList.Add(@object);
      }
      serialized = new fsData((long) i);
      return fsResult.Success;
    }

    public override fsResult TryDeserialize(fsData data, ref object instance, System.Type storageType)
    {
      List<UnityEngine.Object> objectList = this.Serializer.Context.Get<List<UnityEngine.Object>>();
      int asInt64 = (int) data.AsInt64;
      if (asInt64 >= objectList.Count)
        return fsResult.Warn("A Unity Object reference has not been deserialized");
      instance = (object) objectList[asInt64];
      return fsResult.Success;
    }

    public override object CreateInstance(fsData data, System.Type storageType) => (object) null;
  }
}
