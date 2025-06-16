using Cofe.Meta;
using System;
using System.Collections.Generic;

namespace PLVirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
  public class VMFactoryAttribute : TypeAttribute
  {
    private Type type;
    private static Dictionary<Type, Type> types = new Dictionary<Type, Type>();

    public static bool TryGetValue(Type type, out Type result)
    {
      return VMFactoryAttribute.types.TryGetValue(type, out result);
    }

    public VMFactoryAttribute(Type type) => this.type = type;

    public override void ComputeType(Type type) => VMFactoryAttribute.types.Add(this.type, type);
  }
}
