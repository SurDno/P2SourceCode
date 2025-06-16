using System;
using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Weather;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherLayerBlender))]
  public class WeatherLayerBlender_Generated : WeatherLayerBlender, ICloneable, ICopyable
  {
    public object Clone()
    {
      return ServiceCache.Factory.Instantiate(this);
    }

    public void CopyTo(object target2) => ((WeatherLayerBlender_Generated) target2).name = name;
  }
}
