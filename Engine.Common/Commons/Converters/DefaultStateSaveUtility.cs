using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using System;
using System.Collections.Generic;

namespace Engine.Common.Commons.Converters
{
  public static class DefaultStateSaveUtility
  {
    public static void SaveSerialize<T>(IDataWriter writer, string name, T value)
    {
      if ((object) value == null)
        return;
      if (value is ISerializeStateSave serializeStateSave)
      {
        Type type = ProxyFactory.GetType(value.GetType());
        writer.Begin(name, type != typeof (T) ? type : (Type) null, true);
        serializeStateSave.StateSave(writer);
        writer.End(name, true);
      }
      else
        Logger.AddError("Type : " + value.GetType().Name + " is not " + typeof (ISerializeStateSave).Name);
    }

    public static void SaveListSerialize<T>(IDataWriter writer, string name, List<T> value) where T : class
    {
      writer.Begin(name, (Type) null, true);
      foreach (T obj in value)
        DefaultStateSaveUtility.SaveSerialize<T>(writer, "Item", obj);
      writer.End(name, true);
    }
  }
}
