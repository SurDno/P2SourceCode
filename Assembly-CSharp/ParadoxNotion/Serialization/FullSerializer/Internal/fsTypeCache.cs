// Decompiled with JetBrains decompiler
// Type: ParadoxNotion.Serialization.FullSerializer.Internal.fsTypeCache
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;

#nullable disable
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
