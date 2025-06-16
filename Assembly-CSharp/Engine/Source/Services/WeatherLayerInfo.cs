using System;
using Engine.Common.Generator;
using Engine.Common.Weather;
using Inspectors;

namespace Engine.Source.Services;

[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class WeatherLayerInfo {
	[StateSaveProxy] [StateLoadProxy] [Inspected]
	public WeatherLayer Layer;

	[StateSaveProxy] [StateLoadProxy] [Inspected]
	public float Opacity;

	[StateSaveProxy] [StateLoadProxy()] [Inspected]
	public Guid SnapshotTemplateId;
}