using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace AmplifyColor;

[Serializable]
public class VolumeEffectComponent {
	public string componentName;
	public List<VolumeEffectField> fields;

	public VolumeEffectComponent(string name) {
		componentName = name;
		fields = new List<VolumeEffectField>();
	}

	public VolumeEffectField AddField(FieldInfo pi, Component c) {
		return AddField(pi, c, -1);
	}

	public VolumeEffectField AddField(FieldInfo pi, Component c, int position) {
		var volumeEffectField =
			VolumeEffectField.IsValidType(pi.FieldType.FullName) ? new VolumeEffectField(pi, c) : null;
		if (volumeEffectField != null) {
			if (position < 0 || position >= fields.Count)
				fields.Add(volumeEffectField);
			else
				fields.Insert(position, volumeEffectField);
		}

		return volumeEffectField;
	}

	public void RemoveEffectField(VolumeEffectField field) {
		fields.Remove(field);
	}

	public VolumeEffectComponent(Component c, VolumeEffectComponentFlags compFlags)
		: this(compFlags.componentName) {
		foreach (var componentField in compFlags.componentFields)
			if (componentField.blendFlag) {
				var field = c.GetType().GetField(componentField.fieldName);
				var volumeEffectField = VolumeEffectField.IsValidType(field.FieldType.FullName)
					? new VolumeEffectField(field, c)
					: null;
				if (volumeEffectField != null)
					fields.Add(volumeEffectField);
			}
	}

	public void UpdateComponent(Component c, VolumeEffectComponentFlags compFlags) {
		foreach (var componentField in compFlags.componentFields) {
			var fieldFlags = componentField;
			if (fieldFlags.blendFlag && !fields.Exists(s => s.fieldName == fieldFlags.fieldName)) {
				var field = c.GetType().GetField(fieldFlags.fieldName);
				var volumeEffectField = VolumeEffectField.IsValidType(field.FieldType.FullName)
					? new VolumeEffectField(field, c)
					: null;
				if (volumeEffectField != null)
					fields.Add(volumeEffectField);
			}
		}
	}

	public VolumeEffectField FindEffectField(string fieldName) {
		for (var index = 0; index < fields.Count; ++index)
			if (fields[index].fieldName == fieldName)
				return fields[index];
		return null;
	}

	public static FieldInfo[] ListAcceptableFields(Component c) {
		return c == null
			? new FieldInfo[0]
			: c.GetType().GetFields().Where(f => VolumeEffectField.IsValidType(f.FieldType.FullName)).ToArray();
	}

	public string[] GetFieldNames() {
		return fields.Select(r => r.fieldName).ToArray();
	}
}