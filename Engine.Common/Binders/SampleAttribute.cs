using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace Engine.Common.Binders
{
  [AttributeUsage(AttributeTargets.Interface)]
  public class SampleAttribute : TypeAttribute
  {
    public readonly string Name;
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public SampleAttribute(string name) => Name = name;

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
