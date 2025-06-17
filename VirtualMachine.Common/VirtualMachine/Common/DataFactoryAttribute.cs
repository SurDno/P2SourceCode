using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace VirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class DataFactoryAttribute(string typeName) : TypeAttribute 
  {
    private static Dictionary<string, Type> types = new();

    public string TypeName { get; set; } = typeName;

    public override void ComputeType(Type type)
    {
      types.Add(TypeName, type);
    }

    public static Type GetTypeByName(string typeName)
    {
      types.TryGetValue(typeName, out Type typeByName);
      return typeByName;
    }

    public static IEnumerable<KeyValuePair<string, Type>> Items => types;
  }
}
