using System;
using System.Collections.Generic;

namespace Engine.Source.Services
{
  public static class ProfilerUtility
  {
    private static Dictionary<Type, string> types = new Dictionary<Type, string>();

    public static string GetTypeName(Type type)
    {
      string name;
      if (!ProfilerUtility.types.TryGetValue(type, out name))
      {
        name = type.Name;
        ProfilerUtility.types.Add(type, name);
      }
      return name;
    }
  }
}
