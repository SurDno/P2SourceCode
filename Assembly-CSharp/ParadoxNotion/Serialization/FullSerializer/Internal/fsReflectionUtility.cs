// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsReflectionUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
namespace ParadoxNotion.Serialization.FullSerializer.Internal
{
  public static class fsReflectionUtility
  {
    public static Type GetInterface(Type type, Type interfaceType)
    {
      if (interfaceType.Resolve().IsGenericType && !interfaceType.Resolve().IsGenericTypeDefinition)
        throw new ArgumentException("GetInterface requires that if the interface type is generic, then it must be the generic type definition, not a specific generic type instantiation");
      for (; type != (Type) null; type = type.Resolve().BaseType)
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
      return (Type) null;
    }
  }
}
