using System;
using Cofe.Loggers;
using Engine.Common.Components;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS;

[Info("DiseaseComponent", typeof(IDiseaseComponent))]
public class VMDisease : VMEngineComponent<IDiseaseComponent> {
	public const string ComponentName = "DiseaseComponent";

	[Method("Set disease value", "value, delta time", "")]
	public void SetDiseaseValue(float value, GameTime delta) {
		Component.SetDiseaseValue(value, (TimeSpan)delta);
	}

	[Property("Disease value", "", false)]
	public float DiseaseValue {
		get {
			if (Component != null)
				return Component.DiseaseValue;
			Logger.AddError(string.Format("Component {0} engine instance at {1} not inited!!!", Name, Parent.Name));
			return 0.0f;
		}
	}
}