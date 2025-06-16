using System;
using System.Collections.Generic;
using Cofe.Loggers;
using Cofe.Utility;

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
        if (obj1 == null)
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
      if (source == null)
        return default (T);
      if (source is ICloneable cloneable)
        return (T) cloneable.Clone();
      Logger.AddError(TypeUtility.GetTypeName(source.GetType()) + " is not ICloneable");
      return default (T);
    }

    public static T RuntimeOnlyCopy<T>(T source) where T : class
    {
      if (source == null)
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
