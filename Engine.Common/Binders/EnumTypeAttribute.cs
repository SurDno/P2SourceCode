using Cofe.Meta;
using System;
using System.Collections.Generic;

namespace Engine.Common.Binders
{
  [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
  public class EnumTypeAttribute : TypeAttribute
  {
    public readonly string Name;
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();
    private static Dictionary<Type, string> names = new Dictionary<Type, string>();

    public EnumTypeAttribute(string name) => this.Name = name;

    public override void ComputeType(Type type)
    {
      EnumTypeAttribute.types.Add(this.Name, type);
      EnumTypeAttribute.names.Add(type, this.Name);
    }

    public static bool TryGetValue(Type type, out string result)
    {
      return EnumTypeAttribute.names.TryGetValue(type, out result);
    }

    public static bool TryGetValue(string name, out Type result)
    {
      return EnumTypeAttribute.types.TryGetValue(name, out result);
    }
  }
}
