// Decompiled with JetBrains decompiler
// Type: Scripts.Tools.Serializations.Converters.CustomStateSaveUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components.Parameters;
using Scripts.Tools.Serializations.Customs;
using System.Collections.Generic;
using UnityEngine;

#nullable disable
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
      writer.Begin(name, (System.Type) null, true);
      foreach (T obj in value)
      {
        if ((object) obj != null)
          CustomReferenceUtility.SaveReference(writer, "Item", (object) obj);
      }
      writer.End(name, true);
    }

    public static void SaveListParameters(IDataWriter writer, string name, List<IParameter> value)
    {
      writer.Begin(name, (System.Type) null, true);
      for (int index = 0; index < value.Count; ++index)
      {
        IParameter parameter = value[index];
        if (parameter != null && (!(parameter is INeedSave needSave) || needSave.NeedSave))
        {
          if (!(parameter is ISerializeStateSave serializeStateSave))
          {
            Debug.LogError((object) ("Type : " + TypeUtility.GetTypeName(parameter.GetType()) + " is not " + (object) typeof (ISerializeStateSave)));
          }
          else
          {
            System.Type type = ProxyFactory.GetType(serializeStateSave.GetType());
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
      writer.Begin(name, (System.Type) null, true);
      for (int index = 0; index < value.Count; ++index)
      {
        IComponent component = value[index];
        if (component != null && component is INeedSave needSave && needSave.NeedSave)
        {
          if (!(component is ISerializeStateSave serializeStateSave))
          {
            Debug.LogError((object) ("Type : " + TypeUtility.GetTypeName(component.GetType()) + " is not " + (object) typeof (ISerializeStateSave)));
          }
          else
          {
            System.Type type = ProxyFactory.GetType(serializeStateSave.GetType());
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
