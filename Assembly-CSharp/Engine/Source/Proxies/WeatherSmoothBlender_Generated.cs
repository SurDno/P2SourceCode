using System;
using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Blenders;
using Engine.Source.Commons;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WeatherSmoothBlender))]
public class WeatherSmoothBlender_Generated : WeatherSmoothBlender, ICloneable, ICopyable {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		((WeatherSmoothBlender_Generated)target2).name = name;
	}
}