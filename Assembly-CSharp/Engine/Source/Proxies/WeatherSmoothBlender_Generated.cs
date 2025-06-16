using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Blenders;
using Engine.Source.Commons;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (WeatherSmoothBlender))]
  public class WeatherSmoothBlender_Generated : WeatherSmoothBlender, ICloneable, ICopyable
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<WeatherSmoothBlender_Generated>(this);
    }

    public void CopyTo(object target2) => ((EngineObject) target2).name = this.name;
  }
}
