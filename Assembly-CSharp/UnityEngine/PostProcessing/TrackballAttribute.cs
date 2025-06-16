using System;

namespace UnityEngine.PostProcessing;

[AttributeUsage(AttributeTargets.Field)]
public sealed class TrackballAttribute : PropertyAttribute {
	public readonly string method;

	public TrackballAttribute(string method) {
		this.method = method;
	}
}