using System;

namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public static class fsTypeCache
  {
    public static Type GetType(string name) => fsTypeCache.GetType(name, false, (Type) null);

    public static Type GetType(string name, Type fallbackAssignable)
    {
      return fsTypeCache.GetType(name, true, fallbackAssignable);
    }

    private static Type GetType(string name, bool fallbackNoNamespace, Type fallbackAssignable)
    {
      return ReflectionTools.GetType(name, fallbackNoNamespace, fallbackAssignable);
    }
  }
}
