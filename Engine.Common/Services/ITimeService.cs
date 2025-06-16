using System;

namespace Engine.Common.Services;

public interface ITimeService {
	TimeSpan SolarTime { get; set; }

	float SolarTimeFactor { get; set; }

	TimeSpan GameTime { get; set; }

	float GameTimeFactor { get; set; }

	event Action<TimeSpan> GameTimeChangedEvent;

	float DefaultTimeFactor { get; }
}