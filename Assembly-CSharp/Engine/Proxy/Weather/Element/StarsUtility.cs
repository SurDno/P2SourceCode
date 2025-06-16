// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.StarsUtility
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
  public class StarsUtility
  {
    public static void CopyTo(Stars stars)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      stars.Size = tod.Stars.Size;
      stars.Brightness = tod.Stars.Brightness;
    }

    public static void CopyFrom(Stars stars)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.Stars.Size = stars.Size;
      tod.Stars.Brightness = stars.Brightness;
    }
  }
}
