// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.SunUtility
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
  public class SunUtility
  {
    public static void CopyTo(Sun sun)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      sun.Size = tod.Sun.MeshSize;
      sun.Brightness = tod.Sun.MeshBrightness;
      sun.Contrast = tod.Sun.MeshContrast;
    }

    public static void CopyFrom(Sun sun)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Sun.MeshSize = sun.Size;
      tod.Sun.MeshBrightness = sun.Brightness;
      tod.Sun.MeshContrast = sun.Contrast;
    }
  }
}
