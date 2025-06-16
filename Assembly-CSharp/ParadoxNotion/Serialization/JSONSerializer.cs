using System;
using System.Collections.Generic;
using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using Object = UnityEngine.Object;

namespace ParadoxNotion.Serialization
{
  public static class JSONSerializer
  {
    private static Dictionary<string, fsData> cache = new Dictionary<string, fsData>();
    private static object serializerLock = new object();
    private static fsSerializer serializer = new fsSerializer();
    private static bool init;
    public static bool applicationPlaying = true;

    public static string Serialize(
      Type type,
      object value,
      bool pretyJson = false,
      List<Object> objectReferences = null)
    {
      lock (serializerLock)
      {
        if (!init)
        {
          serializer.AddConverter(new fsUnityObjectConverter());
          init = true;
        }
        if (objectReferences != null)
        {
          objectReferences.Clear();
          serializer.Context.Set(objectReferences);
        }
        Type overrideConverterType = typeof (Object).RTIsAssignableFrom(type) ? typeof (fsReflectedConverter) : null;
        fsData data;
        serializer.TrySerialize(type, overrideConverterType, value, out data).AssertSuccess();
        return pretyJson ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
      }
    }

    public static T Deserialize<T>(
      string serializedState,
      List<Object> objectReferences = null,
      T deserialized = null)
    {
      return (T) Deserialize(typeof (T), serializedState, objectReferences, deserialized);
    }

    public static object Deserialize(
      Type type,
      string serializedState,
      List<Object> objectReferences = null,
      object deserialized = null)
    {
      lock (serializerLock)
      {
        if (!init)
        {
          serializer.AddConverter(new fsUnityObjectConverter());
          init = true;
        }
        if (objectReferences != null)
          serializer.Context.Set(objectReferences);
        fsData data = null;
        cache.TryGetValue(serializedState, out data);
        if (data == null)
        {
          data = fsJsonParser.Parse(serializedState);
          cache[serializedState] = data;
        }
        Type overrideConverterType = typeof (Object).RTIsAssignableFrom(type) ? typeof (fsReflectedConverter) : null;
        serializer.TryDeserialize(data, type, overrideConverterType, ref deserialized).AssertSuccess();
        return deserialized;
      }
    }
  }
}
