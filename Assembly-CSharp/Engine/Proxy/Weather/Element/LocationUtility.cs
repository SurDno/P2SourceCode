// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.LocationUtility
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
  public class LocationUtility
  {
    public static void CopyTo(Location location)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      location.Latitude = tod.World.Latitude;
      location.Longitude = tod.World.Longitude;
      location.Utc = tod.World.UTC;
    }

    public static void CopyFrom(Location location)
    {
      TOD_Sky tod = ServiceLocator.GetService<EnvironmentService>().Tod;
      if ((Object) tod == (Object) null)
        return;
      tod.World.Latitude = location.Latitude;
      tod.World.Longitude = location.Longitude;
      tod.World.UTC = location.Utc;
    }
  }
}
