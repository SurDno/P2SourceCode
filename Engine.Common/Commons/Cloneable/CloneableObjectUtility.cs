// Decompiled with JetBrains decompiler
// Type: Engine.Common.Commons.Cloneable.CloneableObjectUtility
// Assembly: Engine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3568A167-18A7-4983-8BC2-C25824901591
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Engine.Common.dll

using Cofe.Loggers;
using Cofe.Utility;
using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Common.Commons.Cloneable
{
  public static class CloneableObjectUtility
  {
    public static void FillListTo(List<string> target, List<string> source)
    {
      target.Clear();
      target.Capacity = source.Count;
      foreach (string str in source)
        target.Add(str);
    }

    public static void FillListTo<T>(List<T> target, List<T> source) where T : struct
    {
      target.Clear();
      target.Capacity = source.Count;
      foreach (T obj in source)
        target.Add(obj);
    }

    public static void CopyListTo<T>(List<T> target, List<T> source) where T : class
    {
      target.Clear();
      target.Capacity = source.Count;
      foreach (T obj1 in source)
      {
        if ((object) obj1 == null)
          target.Add(default (T));
        else if (!(obj1 is ICloneable cloneable))
        {
          Logger.AddError(TypeUtility.GetTypeName(obj1.GetType()) + " is not ICloneable");
        }
        else
        {
          T obj2 = (T) cloneable.Clone();
          target.Add(obj2);
        }
      }
    }

    public static T Clone<T>(T source) where T : class
    {
      if ((object) source == null)
        return default (T);
      if (source is ICloneable cloneable)
        return (T) cloneable.Clone();
      Logger.AddError(TypeUtility.GetTypeName(source.GetType()) + " is not ICloneable");
      return default (T);
    }

    public static T RuntimeOnlyCopy<T>(T source) where T : class
    {
      if ((object) source == null)
        return default (T);
      if (EngineRuntime.IsRuntime)
        return source;
      if (source is ICloneable cloneable)
        return (T) cloneable.Clone();
      Logger.AddError(TypeUtility.GetTypeName(source.GetType()) + " is not ICloneable");
      return default (T);
    }
  }
}
