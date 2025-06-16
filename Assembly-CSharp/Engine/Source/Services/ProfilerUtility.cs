// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.ProfilerUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Services
{
  public static class ProfilerUtility
  {
    private static Dictionary<Type, string> types = new Dictionary<Type, string>();

    public static string GetTypeName(Type type)
    {
      string name;
      if (!ProfilerUtility.types.TryGetValue(type, out name))
      {
        name = type.Name;
        ProfilerUtility.types.Add(type, name);
      }
      return name;
    }
  }
}
