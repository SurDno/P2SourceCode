// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.MoonUtility
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
  public class MoonUtility
  {
    public static void CopyTo(Moon moon)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      moon.Size = tod.Moon.MeshSize;
      moon.Brightness = tod.Moon.MeshBrightness;
      moon.Contrast = tod.Moon.MeshContrast;
    }

    public static void CopyFrom(Moon moon)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Moon.MeshSize = moon.Size;
      tod.Moon.MeshBrightness = moon.Brightness;
      tod.Moon.MeshContrast = moon.Contrast;
    }
  }
}
