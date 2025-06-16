// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.JSONSerializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Serialization.FullSerializer;
using ParadoxNotion.Serialization.FullSerializer.Internal;
using System.Collections.Generic;

#nullable disable
namespace ParadoxNotion.Serialization
{
  public static class JSONSerializer
  {
    private static Dictionary<string, fsData> cache = new Dictionary<string, fsData>();
    private static object serializerLock = new object();
    private static fsSerializer serializer = new fsSerializer();
    private static bool init = false;
    public static bool applicationPlaying = true;

    public static string Serialize(
      System.Type type,
      object value,
      bool pretyJson = false,
      List<UnityEngine.Object> objectReferences = null)
    {
      lock (JSONSerializer.serializerLock)
      {
        if (!JSONSerializer.init)
        {
          JSONSerializer.serializer.AddConverter((fsBaseConverter) new fsUnityObjectConverter());
          JSONSerializer.init = true;
        }
        if (objectReferences != null)
        {
          objectReferences.Clear();
          JSONSerializer.serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
        }
        System.Type overrideConverterType = typeof (UnityEngine.Object).RTIsAssignableFrom(type) ? typeof (fsReflectedConverter) : (System.Type) null;
        fsData data;
        JSONSerializer.serializer.TrySerialize(type, overrideConverterType, value, out data).AssertSuccess();
        return pretyJson ? fsJsonPrinter.PrettyJson(data) : fsJsonPrinter.CompressedJson(data);
      }
    }

    public static T Deserialize<T>(
      string serializedState,
      List<UnityEngine.Object> objectReferences = null,
      T deserialized = null)
    {
      return (T) JSONSerializer.Deserialize(typeof (T), serializedState, objectReferences, (object) deserialized);
    }

    public static object Deserialize(
      System.Type type,
      string serializedState,
      List<UnityEngine.Object> objectReferences = null,
      object deserialized = null)
    {
      lock (JSONSerializer.serializerLock)
      {
        if (!JSONSerializer.init)
        {
          JSONSerializer.serializer.AddConverter((fsBaseConverter) new fsUnityObjectConverter());
          JSONSerializer.init = true;
        }
        if (objectReferences != null)
          JSONSerializer.serializer.Context.Set<List<UnityEngine.Object>>(objectReferences);
        fsData data = (fsData) null;
        JSONSerializer.cache.TryGetValue(serializedState, out data);
        if (data == (fsData) null)
        {
          data = fsJsonParser.Parse(serializedState);
          JSONSerializer.cache[serializedState] = data;
        }
        System.Type overrideConverterType = typeof (UnityEngine.Object).RTIsAssignableFrom(type) ? typeof (fsReflectedConverter) : (System.Type) null;
        JSONSerializer.serializer.TryDeserialize(data, type, overrideConverterType, ref deserialized).AssertSuccess();
        return deserialized;
      }
    }
  }
}
