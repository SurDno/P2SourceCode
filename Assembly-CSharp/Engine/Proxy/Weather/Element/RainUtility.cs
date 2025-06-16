// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.RainUtility
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Rain;
using UnityEngine;

#nullable disable
namespace Engine.Proxy.Weather.Element
{
  public class RainUtility
  {
    public static void CopyTo(Engine.Impl.Weather.Element.Rain rain)
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null)
        return;
      rain.Intensity = instance.rainIntensity;
      rain.TerrainDryTime = instance.terrainDryTime;
      rain.TerrainFillTime = instance.terrainFillTime;
      rain.PuddlesDryTime = instance.puddleDryTime;
      rain.PuddlesFillTime = instance.puddleFillTime;
      rain.Direction = instance.windVector;
    }

    public static void CopyFrom(Engine.Impl.Weather.Element.Rain rain)
    {
      RainManager instance = RainManager.Instance;
      if ((Object) instance == (Object) null)
        return;
      instance.rainIntensity = rain.Intensity;
      instance.terrainDryTime = rain.TerrainDryTime;
      instance.terrainFillTime = rain.TerrainFillTime;
      instance.puddleDryTime = rain.PuddlesDryTime;
      instance.puddleFillTime = rain.PuddlesFillTime;
      instance.windVector = rain.Direction;
    }
  }
}
