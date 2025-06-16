using Engine.Common.Weather;

namespace Engine.Common.Services;

public interface IWeatherController {
	IWeatherLayerBlenderItem GetItem(WeatherLayer layer);
}