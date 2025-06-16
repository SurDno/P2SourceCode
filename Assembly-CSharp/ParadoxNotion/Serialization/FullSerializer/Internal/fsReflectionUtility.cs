using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public static class fsReflectionUtility
  {
    public static Type GetInterface(Type type, Type interfaceType)
    {
      if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
        throw new ArgumentException("GetInterface requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
      for (; type != null; type = type.Resolve().BaseType)
      {
        foreach (Type type1 in type.GetInterfaces())
        {
          if (type1.Resolve().IsGenericType)
          {
            if (interfaceType == type1.GetGenericTypeDefinition())
              return type1;
          }
          else if (interfaceType == type1)
            return type1;
        }
      }
      return null;
    }
  }
}
