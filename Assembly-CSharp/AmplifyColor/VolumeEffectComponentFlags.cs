using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor;

[Serializable]
public class VolumeEffectComponentFlags {
	public string componentName;
	public List<VolumeEffectFieldFlags> componentFields;
	public bool blendFlag;

	public VolumeEffectComponentFlags(string name) {
		componentName = name;
		componentFields = new List<VolumeEffectFieldFlags>();
	}

	public VolumeEffectComponentFlags(VolumeEffectComponent comp)
		: this(comp.componentName) {
		blendFlag = true;
		foreach (var field in comp.fields)
			if (VolumeEffectField.IsValidType(field.fieldType))
				componentFields.Add(new VolumeEffectFieldFlags(field));
	}

	public VolumeEffectComponentFlags(Component c)
		: this(string.Concat(c.GetType())) {
		foreach (var field in c.GetType().GetFields())
			if (VolumeEffectField.IsValidType(field.FieldType.FullName))
				componentFields.Add(new VolumeEffectFieldFlags(field));
	}

	public void UpdateComponentFlags(VolumeEffectComponent comp) {
		foreach (var field1 in comp.fields) {
			var field = field1;
			if (componentFields.Find(s => s.fieldName == field.fieldName) == null &&
			    VolumeEffectField.IsValidType(field.fieldType))
				componentFields.Add(new VolumeEffectFieldFlags(field));
		}
	}

	public void UpdateComponentFlags(Component c) {
		foreach (var field in c.GetType().GetFields()) {
			var pi = field;
			if (!componentFields.Exists(s => s.fieldName == pi.Name) &&
			    VolumeEffectField.IsValidType(pi.FieldType.FullName))
				componentFields.Add(new VolumeEffectFieldFlags(pi));
		}
	}

	public string[] GetFieldNames() {
		return componentFields.Where(r => r.blendFlag).Select(r => r.fieldName).ToArray();
	}
}