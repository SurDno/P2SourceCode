// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.ThunderStormUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;
using UnityEngine;

#nullable disable
namespace Engine.Proxy.Weather.Element
{
  public static class ThunderStormUtility
  {
    public static void CopyTo(ThunderStorm thunderStorm)
    {
      if (!((Object) ServiceLocator.GetService<EnvironmentService>().Tod == (Object) null))
        ;
    }

    public static void CopyFrom(ThunderStorm thunderStorm)
    {
      if (!((Object) ServiceLocator.GetService<EnvironmentService>().Tod == (Object) null))
        ;
    }
  }
}
