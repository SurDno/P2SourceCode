// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.WindUtility
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
  public class WindUtility
  {
    public static void CopyTo(Wind wind)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      if ((Object) tod.Components.Animation == (Object) null)
        return;
      TOD_Animation component = tod.Components.Animation.GetComponent<TOD_Animation>();
      wind.Degrees = component.WindDegrees;
      wind.Speed = component.WindSpeed;
    }

    public static void CopyFrom(Wind wind)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      if ((Object) tod.Components.Animation == (Object) null)
        return;
      TOD_Animation component = tod.Components.Animation.GetComponent<TOD_Animation>();
      component.WindDegrees = wind.Degrees;
      component.WindSpeed = wind.Speed;
    }
  }
}
