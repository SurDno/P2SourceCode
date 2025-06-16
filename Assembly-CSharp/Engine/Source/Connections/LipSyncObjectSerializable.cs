using System;
using System.IO;
using Cofe.Serializations.Converters;
using Engine.Source.Commons;
using UnityEngine;

namespace Engine.Source.Connections;

[Serializable]
public struct LipSyncObjectSerializable : IEngineSerializable {
	[SerializeField] private string id;

	public Guid Id {
		get => DefaultConverter.ParseGuid(id);
		set => id = value != Guid.Empty ? value.ToString() : "";
	}

	public Type Type => typeof(LipSyncObject);

	public LipSyncObject Value {
		get => TemplateUtility.GetTemplate<LipSyncObject>(Id);
		set => Id = value != null ? value.Id : Guid.Empty;
	}

	public override int GetHashCode() {
		return string.IsNullOrEmpty(id) ? 0 : id.GetHashCode();
	}

	public override bool Equals(object a) {
		return a is LipSyncObjectSerializable objectSerializable && this == objectSerializable;
	}

	public static bool operator ==(LipSyncObjectSerializable a, LipSyncObjectSerializable b) {
		return a.id == b.id;
	}

	public static bool operator !=(LipSyncObjectSerializable a, LipSyncObjectSerializable b) {
		return !(a == b);
	}

	public override string ToString() {
		var lipSyncObject = Value;
		return lipSyncObject != null ? Path.GetFileNameWithoutExtension(lipSyncObject.Source) : "";
	}
}