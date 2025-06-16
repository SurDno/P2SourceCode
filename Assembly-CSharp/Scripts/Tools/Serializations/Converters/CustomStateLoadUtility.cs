// Decompiled with JetBrains decompiler
// Type: Scripts.Tools.Serializations.Converters.CustomStateLoadUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Scripts.Tools.Serializations.Customs;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace Scripts.Tools.Serializations.Converters
{
  public static class CustomStateLoadUtility
  {
    public static T LoadReference<T>(IDataReader reader, string name, T value) where T : class
    {
      return (T) CustomReferenceUtility.LoadReference(reader, name, typeof (T));
    }

    public static List<T> LoadListReferences<T>(IDataReader reader, string name, List<T> value) where T : class
    {
      value.Clear();
      foreach (IDataReader child in reader.GetChild(name).GetChilds())
      {
        T obj = (T) CustomReferenceUtility.LoadReference(child, typeof (T));
        value.Add(obj);
      }
      return value;
    }

    public static List<IParameter> LoadListParameters(
      IDataReader reader,
      string name,
      List<IParameter> value)
    {
      foreach (IDataReader child1 in reader.GetChild(name).GetChilds())
      {
        IDataReader child2 = child1.GetChild("Name");
        if (child2 == null)
        {
          Logger.AddError("Parameter name not found, context : " + reader.GetContext());
        }
        else
        {
          ParameterNameEnum parameterName;
          DefaultConverter.TryParseEnum<ParameterNameEnum>(child2.Read(), out parameterName);
          IParameter parameter = value.FirstOrDefault<IParameter>((Func<IParameter, bool>) (o => o.Name == parameterName));
          if (parameter != null)
          {
            if (!(parameter is ISerializeStateLoad serializeStateLoad))
            {
              Logger.AddError("Type : " + TypeUtility.GetTypeName(parameter.GetType()) + " is not " + (object) typeof (ISerializeStateLoad));
            }
            else
            {
              Type type = ProxyFactory.GetType(parameter.GetType());
              serializeStateLoad.StateLoad(child1, type);
            }
          }
        }
      }
      return value;
    }

    public static List<IComponent> LoadListComponents(
      IDataReader reader,
      string name,
      List<IComponent> value)
    {
      IDataReader child1 = reader.GetChild(name);
      if (child1 != null)
      {
        foreach (IDataReader child2 in child1.GetChilds())
        {
          string dataType = child2.GetDataType();
          for (int index = 0; index < value.Count; ++index)
          {
            IComponent component = value[index];
            if (component != null)
            {
              Type type = ProxyFactory.GetType(component.GetType());
              if (type.Name == dataType)
              {
                if (!(component is ISerializeStateLoad serializeStateLoad))
                  Logger.AddError("Type : " + TypeUtility.GetTypeName(component.GetType()) + " is not " + (object) typeof (ISerializeStateLoad));
                else
                  serializeStateLoad.StateLoad(child2, type);
              }
            }
          }
        }
      }
      return value;
    }
  }
}
