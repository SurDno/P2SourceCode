using Cofe.Meta;
using System;
using System.Collections.Generic;

namespace VirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
  public class DataFactoryAttribute : TypeAttribute
  {
    private static Dictionary<string, Type> types = new Dictionary<string, Type>();

    public string TypeName { get; set; }

    public DataFactoryAttribute(string typeName) => this.TypeName = typeName;

    public override void ComputeType(Type type)
    {
      DataFactoryAttribute.types.Add(this.TypeName, type);
    }

    public static Type GetTypeByName(string typeName)
    {
      Type typeByName;
      DataFactoryAttribute.types.TryGetValue(typeName, out typeByName);
      return typeByName;
    }

    public static IEnumerable<KeyValuePair<string, Type>> Items
    {
      get => (IEnumerable<KeyValuePair<string, Type>>) DataFactoryAttribute.types;
    }
  }
}
