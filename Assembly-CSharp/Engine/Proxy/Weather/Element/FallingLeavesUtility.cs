using Engine.Common.Services;
using Engine.Impl.Weather.Element;
using Engine.Services.Engine;
using UnityEngine;

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
