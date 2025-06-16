using System;
using System.Collections.Generic;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Scripts.Tools.Serializations.Customs;
using UnityEngine;

namespace Scripts.Tools.Serializations.Converters
{
  public static class CustomStateSaveUtility
  {
    public static void SaveReference(IDataWriter writer, string name, object value)
    {
      if (value == null)
        return;
      CustomReferenceUtility.SaveReference(writer, name, value);
    }

    public static void SaveListReferences<T>(IDataWriter writer, string name, List<T> value) where T : class
    {
      writer.Begin(name, null, true);
      foreach (T obj in value)
      {
        if (obj != null)
          CustomReferenceUtility.SaveReference(writer, "Item", obj);
      }
      writer.End(name, true);
    }

    public static void SaveListParameters(IDataWriter writer, string name, List<IParameter> value)
    {
      writer.Begin(name, null, true);
      for (int index = 0; index < value.Count; ++index)
      {
        IParameter parameter = value[index];
        if (parameter != null && (!(parameter is INeedSave needSave) || needSave.NeedSave))
        {
          if (!(parameter is ISerializeStateSave serializeStateSave))
          {
            Debug.LogError("Type : " + TypeUtility.GetTypeName(parameter.GetType()) + " is not " + typeof (ISerializeStateSave));
          }
          else
          {
            Type type = ProxyFactory.GetType(serializeStateSave.GetType());
            writer.Begin("Item", type, true);
            serializeStateSave.StateSave(writer);
            writer.End("Item", true);
          }
        }
      }
      writer.End(name, true);
    }

    public static void SaveListComponents(IDataWriter writer, string name, List<IComponent> value)
    {
      writer.Begin(name, null, true);
      for (int index = 0; index < value.Count; ++index)
      {
        IComponent component = value[index];
        if (component != null && component is INeedSave needSave && needSave.NeedSave)
        {
          if (!(component is ISerializeStateSave serializeStateSave))
          {
            Debug.LogError("Type : " + TypeUtility.GetTypeName(component.GetType()) + " is not " + typeof (ISerializeStateSave));
          }
          else
          {
            Type type = ProxyFactory.GetType(serializeStateSave.GetType());
            writer.Begin("Item", type, true);
            serializeStateSave.StateSave(writer);
            writer.End("Item", true);
          }
        }
      }
      writer.End(name, true);
    }
  }
}
