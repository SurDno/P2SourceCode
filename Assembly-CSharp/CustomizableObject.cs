using System;
using System.Linq;
using Engine.Common;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

public class CustomizableObject : MonoBehaviour, IEntityAttachable, IChangeParameterListener {
	[SerializeField] private Preset[] presets;
	private int presetIndex;
	private IParameter<int> parameter;

	[Inspected(Mutable = true)]
	[Inspected(Mutable = true, Mode = ExecuteMode.Edit)]
	public int PresetIndex {
		get => presetIndex;
		set {
			presetIndex = value;
			if (!Application.isPlaying)
				return;
			OnInvalidate();
		}
	}

	private void OnInvalidate() {
		var preset = presets.ElementAtOrDefault(presetIndex);
		if (preset == null)
			return;
		for (var index = 0; index < preset.Infos.Length; ++index) {
			var info = preset.Infos[index];
			if (!(info.Renderer == null) && info.Materials != null)
				info.Renderer.materials = info.Materials;
		}
	}

	public void Attach(IEntity owner) {
		var component = owner.GetComponent<ParametersComponent>();
		if (component == null)
			return;
		var byName = component.GetByName<int>(ParameterNameEnum.Customization);
		if (byName != null) {
			byName.AddListener(this);
			OnParameterChanged(byName);
		}
	}

	public void Detach() {
		if (parameter == null)
			return;
		parameter.RemoveListener(this);
		parameter = null;
	}

	public void OnParameterChanged(IParameter parameter) {
		if (presets.Length == 0)
			return;
		PresetIndex = ((IParameter<int>)parameter).Value % presets.Length;
	}

	[Serializable]
	public class SubMeshInfo {
		public Renderer Renderer;
		public Material[] Materials;
	}

	[Serializable]
	public class Preset {
		public SubMeshInfo[] Infos;
	}
}