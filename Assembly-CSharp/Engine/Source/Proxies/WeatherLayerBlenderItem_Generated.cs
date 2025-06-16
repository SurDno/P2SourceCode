using System;
using Cofe.Proxies;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Weather;

namespace Engine.Source.Proxies;

[FactoryProxy(typeof(WeatherLayerBlenderItem))]
public class WeatherLayerBlenderItem_Generated : WeatherLayerBlenderItem, ICloneable, ICopyable {
	public object Clone() {
		return ServiceCache.Factory.Instantiate(this);
	}

	public void CopyTo(object target2) {
		((WeatherLayerBlenderItem_Generated)target2).name = name;
	}
}