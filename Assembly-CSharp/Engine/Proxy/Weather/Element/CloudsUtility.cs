// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.CloudsUtility
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
  public class CloudsUtility
  {
    public static void CopyTo(Clouds clouds)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      clouds.Size = tod.Clouds.Size;
      clouds.Opacity = tod.Clouds.Opacity;
      clouds.Coverage = tod.Clouds.Coverage;
      clouds.Sharpness = tod.Clouds.Sharpness;
      clouds.Attenuation = tod.Clouds.Attenuation;
      clouds.Saturation = tod.Clouds.Saturation;
      clouds.Scattering = tod.Clouds.Scattering;
      clouds.Brightness = tod.Clouds.Brightness;
    }

    public static void CopyFrom(Clouds clouds)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Clouds.Size = clouds.Size;
      tod.Clouds.Opacity = clouds.Opacity;
      tod.Clouds.Coverage = clouds.Coverage;
      tod.Clouds.Sharpness = clouds.Sharpness;
      tod.Clouds.Attenuation = clouds.Attenuation;
      tod.Clouds.Saturation = clouds.Saturation;
      tod.Clouds.Scattering = clouds.Scattering;
      tod.Clouds.Brightness = clouds.Brightness;
    }
  }
}
