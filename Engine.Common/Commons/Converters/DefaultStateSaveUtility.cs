using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;

namespace Engine.Common.Commons.Converters
{
  public static class DefaultStateSaveUtility
  {
    public static void SaveSerialize<T>(IDataWriter writer, string name, T value)
    {
      if (value == null)
        return;
      if (value is ISerializeStateSave serializeStateSave)
      {
        Type type = ProxyFactory.GetType(value.GetType());
        writer.Begin(name, type != typeof (T) ? type : null, true);
        serializeStateSave.StateSave(writer);
        writer.End(name, true);
      }
      else
        Logger.AddError("Type : " + value.GetType().Name + " is not " + typeof (ISerializeStateSave).Name);
    }

    public static void SaveListSerialize<T>(IDataWriter writer, string name, List<T> value) where T : class
    {
      writer.Begin(name, null, true);
      foreach (T obj in value)
        SaveSerialize(writer, "Item", obj);
      writer.End(name, true);
    }
  }
}
