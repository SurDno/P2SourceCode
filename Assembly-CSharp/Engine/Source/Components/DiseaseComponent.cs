using System;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.Services.Factories;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Components;

[Factory(typeof(IDiseaseComponent))]
[GenerateProxy(TypeEnum.Cloneable | TypeEnum.Copyable | TypeEnum.DataRead | TypeEnum.DataWrite | TypeEnum.StateSave |
               TypeEnum.StateLoad)]
public class DiseaseComponent :
	EngineComponent,
	IDiseaseComponent,
	IComponent,
	IUpdatable,
	INeedSave {
	[StateSaveProxy] [StateLoadProxy()] [Inspected]
	protected float diseaseValue;

	[Inspected] private float startDiseaseValue;
	[Inspected] private float currentDiseaseValue;
	[Inspected] private TimeSpan startTime;
	[Inspected] private TimeSpan endTime;
	[Inspected] private bool update;

	public event Action<float> OnCurrentDiseaseValueChanged;

	public override void OnRemoved() {
		base.OnRemoved();
		if (!update)
			return;
		update = false;
		InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable(this);
	}

	public void SetDiseaseValue(float value, TimeSpan deltaTime) {
		startTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
		if (deltaTime.Ticks <= 0L) {
			endTime = startTime;
			currentDiseaseValue = value;
			startDiseaseValue = value;
			if (update) {
				update = false;
				InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable(this);
			}
		} else {
			endTime = startTime + deltaTime;
			currentDiseaseValue = diseaseValue;
			startDiseaseValue = diseaseValue;
			if (!update) {
				update = true;
				InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.AddUpdatable(this);
			}
		}

		diseaseValue = value;
		var diseaseValueChanged = OnCurrentDiseaseValueChanged;
		if (diseaseValueChanged == null)
			return;
		diseaseValueChanged(currentDiseaseValue);
	}

	public float DiseaseValue => diseaseValue;

	public float CurrentDiseaseValue => currentDiseaseValue;

	public bool NeedSave {
		get {
			if (!(Owner.Template is IEntity template)) {
				Debug.LogError("Template not found, owner : " + Owner.GetInfo());
				return true;
			}

			var component = template.GetComponent<DiseaseComponent>();
			if (component == null) {
				Debug.LogError(GetType().Name + " not found, owner : " + Owner.GetInfo());
				return true;
			}

			return diseaseValue != (double)component.diseaseValue;
		}
	}

	public void ComputeUpdate() {
		if (!update)
			return;
		var absoluteGameTime = ServiceLocator.GetService<TimeService>().AbsoluteGameTime;
		if (absoluteGameTime >= endTime) {
			currentDiseaseValue = diseaseValue;
			update = false;
			InstanceByRequest<UpdateService>.Instance.DiseaseUpdater.RemoveUpdatable(this);
			var diseaseValueChanged = OnCurrentDiseaseValueChanged;
			if (diseaseValueChanged == null)
				return;
			diseaseValueChanged(currentDiseaseValue);
		} else {
			var timeSpan = absoluteGameTime - startTime;
			double ticks1 = timeSpan.Ticks;
			timeSpan = endTime - startTime;
			double ticks2 = timeSpan.Ticks;
			currentDiseaseValue = Mathf.Lerp(startDiseaseValue, diseaseValue, Mathf.Clamp01((float)(ticks1 / ticks2)));
			var diseaseValueChanged = OnCurrentDiseaseValueChanged;
			if (diseaseValueChanged == null)
				return;
			diseaseValueChanged(currentDiseaseValue);
		}
	}

	[OnLoaded]
	private void OnLoaded() {
		currentDiseaseValue = diseaseValue;
		var diseaseValueChanged = OnCurrentDiseaseValueChanged;
		if (diseaseValueChanged == null)
			return;
		diseaseValueChanged(currentDiseaseValue);
	}
}