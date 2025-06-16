using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using System;
using System.Collections.Generic;

namespace Engine.Common.Commons.Converters
{
  public static class DefaultStateLoadUtility
  {
    public static T ReadSerialize<T>(IDataReader reader, string name) where T : class
    {
      IDataReader child = reader.GetChild(name);
      if (child == null)
        return default (T);
      Type realType = MappingUtility.GetRealType(child, typeof (T));
      T obj = (T) ProxyFactory.Create(realType);
      if (!(obj is ISerializeStateLoad serializeStateLoad))
      {
        Logger.AddError("Type : " + TypeUtility.GetTypeName(obj.GetType()) + " is not " + (object) typeof (ISerializeStateLoad));
        return default (T);
      }
      serializeStateLoad.StateLoad(child, realType);
      return obj;
    }

    public static List<T> ReadListSerialize<T>(IDataReader reader, string name, List<T> value)
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        Type realType = MappingUtility.GetRealType(child, typeof (T));
        T obj = (T) ProxyFactory.Create(realType);
        if (!(obj is ISerializeStateLoad serializeStateLoad))
        {
          Logger.AddError("Type : " + TypeUtility.GetTypeName(obj.GetType()) + " is not " + (object) typeof (ISerializeStateLoad));
        }
        else
        {
          serializeStateLoad.StateLoad(child, realType);
          value.Add(obj);
        }
      }
      return value;
    }
  }
}
