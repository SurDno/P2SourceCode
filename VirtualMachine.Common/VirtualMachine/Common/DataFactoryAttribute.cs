using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace VirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class DataFactoryAttribute : TypeAttribute
  {
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();

    public string TypeName { get; set; }

    public DataFactoryAttribute(string typeName) => TypeName = typeName;

    public override void ComputeType(Type type)
    {
      types.Add(TypeName, type);
    }

    public static Type GetTypeByName(string typeName)
    {
      Type typeByName;
      types.TryGetValue(typeName, out typeByName);
      return typeByName;
    }

    public static IEnumerable<KeyValuePair<string, Type>> Items
    {
      get => types;
    }
  }
}
