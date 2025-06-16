using System;
using Cofe.Utility;
using Inspectors;
using Object = UnityEngine.Object;

namespace Engine.Source.Connections;

public struct UnitySubAsset<T> : IUnityAsset where T : Object {
	private Guid id;
	private string name;

	[Inspected] public Guid Id => id;

	[Inspected] public string Name => name;

	public UnitySubAsset(Guid id, string name) {
		this.id = id;
		this.name = name;
	}

	public T Value => UnityAssetUtility.GetValue<T>(id, name);

	public override int GetHashCode() {
		var hashCode = id.GetHashCode();
		if (!name.IsNullOrEmpty())
			hashCode ^= name.GetHashCode();
		return hashCode;
	}

	public override bool Equals(object a) {
		return a is UnitySubAsset<T> unitySubAsset && this == unitySubAsset;
	}

	public static bool operator ==(UnitySubAsset<T> a, UnitySubAsset<T> b) {
		return a.id == b.id && (a.name.IsNullOrEmpty() == b.name.IsNullOrEmpty() || a.name == b.name);
	}

	public static bool operator !=(UnitySubAsset<T> a, UnitySubAsset<T> b) {
		return !(a == b);
	}
}