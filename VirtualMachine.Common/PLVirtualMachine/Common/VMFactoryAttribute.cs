using System;
using System.Collections.Generic;
using Cofe.Meta;

namespace PLVirtualMachine.Common
{
  [AttributeUsage(AttributeTargets.Class, Inherited = false)]
  public class VMFactoryAttribute(Type type) : TypeAttribute 
  {
    private static Dictionary<Type, Type> types = new();

    public static bool TryGetValue(Type type, out Type result)
    {
      return types.TryGetValue(type, out result);
    }

    public override void ComputeType(Type type1) => types.Add(type, type1);
  }
}
