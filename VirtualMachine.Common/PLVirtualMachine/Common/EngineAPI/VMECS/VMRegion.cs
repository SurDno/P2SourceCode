using System;
using Cofe.Loggers;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("Region", typeof(IRegionComponent))]
public class VMRegion : VMEngineComponent<IRegionComponent> {
	public const string ComponentName = "Region";

	[Property("RegionDiseaseLevel", "")]
	public int RegionDiseaseLevel {
		get {
			if (Component != null)
				return Component.DiseaseLevel.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.DiseaseLevel.Value = value;
		}
	}

	[Property("Reputation", "")]
	public float Reputation {
		get {
			if (Component != null)
				return Component.Reputation.Value;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
		set {
			if (Component == null)
				Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			else
				Component.Reputation.Value = value;
		}
	}

	[Property("RegionIndex", "", true, 0)] public int RegionIndex { get; set; }

	public override void Clear() {
		if (!InstanceValid)
			return;
		Component.DiseaseLevel.ChangeValueEvent -= OnDiseaseLevelChanged;
		Component.Reputation.ChangeValueEvent -= OnReputationChanged;
		base.Clear();
	}

	protected override void Init() {
		if (IsTemplate)
			return;
		Component.DiseaseLevel.ChangeValueEvent += OnDiseaseLevelChanged;
		Component.Reputation.ChangeValueEvent += OnReputationChanged;
	}

	[Event("Reputation changed", "level of disease")]
	public event Action<float> ReputationChanged;

	public void OnReputationChanged(float value) {
		var reputationChanged = ReputationChanged;
		if (reputationChanged != null)
			reputationChanged(value);
		if (VMGameComponent.Instance == null)
			return;
		VMGameComponent.Instance.OnRegionReputationChanged(Component, value);
	}

	[Event("Disease level changed", "level of disease")]
	public event Action<int> DiseaseLevelChanged;

	public void OnDiseaseLevelChanged(int value) {
		var diseaseLevelChanged = DiseaseLevelChanged;
		if (diseaseLevelChanged != null)
			diseaseLevelChanged(value);
		VMGameComponent.Instance.OnRegionDiseaseLevelChanged(Component, value);
	}
}