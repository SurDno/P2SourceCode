﻿using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Common.Binders
{
  [AttributeUsage(AttributeTargets.Interface)]
  public class SampleAttribute(string name) : TypeAttribute 
  {
    public readonly string Name = name;
    private static Dictionary<string, Type> types = new();
    private static Dictionary<Type, string> names = new();

    public override void ComputeType(Type type)
    {
      RegisterSampleType(Name, type);
    }

    private static void RegisterSampleType(string name, Type type)
    {
      types.Add(name, type);
      names.Add(type, name);
    }

    public static bool TryGetValue(Type type, out string result)
    {
      return names.TryGetValue(type, out result);
    }

    public static bool TryGetValue(string name, out Type result)
    {
      return types.TryGetValue(name, out result);
    }
  }
}
