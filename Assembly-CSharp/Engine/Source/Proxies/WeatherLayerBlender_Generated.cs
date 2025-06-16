using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Weather;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherLayerBlender))]
  public class WeatherLayerBlender_Generated : WeatherLayerBlender, ICloneable, ICopyable
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<WeatherLayerBlender_Generated>(this);
    }

    public void CopyTo(object target2) => ((EngineObject) target2).name = this.name;
  }
}
