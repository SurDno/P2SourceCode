using System;
using System.Collections;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services;
using Engine.Source.Services.Saves;
using Inspectors;
using Scripts.Data;
using UnityEngine;

namespace Engine.Impl.Services;

[RuntimeService(typeof(TimeService), typeof(ITimeService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class TimeService : ITimeService, IUpdatable, IInitialisable, ISavesController {
	public const float defaultTimeFactor = 12f;
	private float solarTimeFactor = 12f;
	private float gameTimeFactor = 12f;
	private TimeSpan solarTime;
	private TimeSpan gameTime;
	private TimeSpan absoluteGameTime;
	private TimeSpan realTime;
	[Inspected] private TimeSpan demoTime = new(2, 1, 0, 0);

	[Inspected]
	public TimeSpan SolarTime {
		get => solarTime;
		set => solarTime = value;
	}

	[Inspected]
	public float SolarTimeFactor {
		get => solarTimeFactor;
		set => solarTimeFactor = value;
	}

	[Inspected]
	public TimeSpan GameTime {
		get => gameTime;
		set => gameTime = value;
	}

	[Inspected]
	public float GameTimeFactor {
		get => gameTimeFactor;
		set => gameTimeFactor = value;
	}

	public event Action<TimeSpan> GameTimeChangedEvent;

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected]
	public TimeSpan AbsoluteGameTime {
		get => absoluteGameTime;
		protected set => absoluteGameTime = value;
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected]
	public TimeSpan RealTime {
		get => realTime;
		protected set => realTime = value;
	}

	[Inspected] public float DefaultTimeFactor => 12f;

	public void SetGameTime(TimeSpan time) {
		var timeChangedEvent = GameTimeChangedEvent;
		if (timeChangedEvent == null)
			return;
		timeChangedEvent(time);
	}

	public TimeSpan Delta { get; private set; }

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
		SolarTime = ScriptableObjectInstance<BuildSettings>.Instance.MainMenuSolarTime.Value;
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		if (InstanceByRequest<EngineApplication>.Instance.IsPaused ||
		    !InstanceByRequest<EngineApplication>.Instance.ViewEnabled)
			Delta = TimeSpan.Zero;
		else {
			Delta = TimeSpan.FromSeconds(Time.deltaTime * (double)GameTimeFactor);
			absoluteGameTime += Delta;
			solarTime += TimeSpan.FromSeconds(Time.deltaTime * (double)SolarTimeFactor);
			realTime += TimeSpan.FromSeconds(Time.deltaTime);
			ComputeDemo();
		}
	}

	public IEnumerator Load(IErrorLoadingHandler errorHandler) {
		var data = InstanceByRequest<GameDataService>.Instance.GetCurrentGameData();
		SolarTime = data.SolarTime.Value;
		yield break;
	}

	public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler) {
		var node = element[TypeUtility.GetTypeName(GetType())];
		if (node == null)
			errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
		else {
			var reader = new XmlNodeDataReader(node, context);
			((ISerializeStateLoad)this).StateLoad(reader, GetType());
			yield break;
		}
	}

	public void Unload() {
		Delta = TimeSpan.Zero;
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}

	private void ComputeDemo() {
		if (!ScriptableObjectInstance<BuildData>.Instance.Demo || !(GameTime > demoTime))
			return;
		ServiceLocator.GetService<GameLauncher>().ExitToMainMenu();
	}
}