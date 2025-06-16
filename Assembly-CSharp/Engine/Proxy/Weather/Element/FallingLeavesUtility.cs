// Decompiled with JetBrains decompiler
// Type: Engine.Proxy.Weather.Element.FallingLeavesUtility
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
  public class FallingLeavesUtility
  {
    public static void CopyTo(FallingLeaves fallingLeaves)
    {
      LeafManager fallingLeaves1 = ServiceLocator.GetService<EnvironmentService>().FallingLeaves;
      if ((Object) fallingLeaves1 == (Object) null)
        return;
      fallingLeaves.PoolCapacity = fallingLeaves1.poolCapacity;
      fallingLeaves.Radius = fallingLeaves1.radius;
      fallingLeaves.MinLandingNormalY = fallingLeaves1.minLandingNormalY;
      fallingLeaves.Deviation = fallingLeaves1.deviation;
      fallingLeaves.Rate = fallingLeaves1.rate;
    }

    public static void CopyFrom(FallingLeaves fallingLeaves)
    {
      LeafManager fallingLeaves1 = ServiceLocator.GetService<EnvironmentService>().FallingLeaves;
      if ((Object) fallingLeaves1 == (Object) null)
        return;
      fallingLeaves1.poolCapacity = fallingLeaves.PoolCapacity;
      fallingLeaves1.radius = fallingLeaves.Radius;
      fallingLeaves1.minLandingNormalY = fallingLeaves.MinLandingNormalY;
      fallingLeaves1.deviation = fallingLeaves.Deviation;
      fallingLeaves1.rate = fallingLeaves.Rate;
    }
  }
}
