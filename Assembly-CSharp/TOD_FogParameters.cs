using System;
using UnityEngine;

[Serializable]
public class TOD_FogParameters {
	[Tooltip("Fog color mode.")] public TOD_FogType Mode = TOD_FogType.Color;

	[Tooltip(
		"Fog color sampling height.\n = 0 fog is atmosphere color at horizon.\n = 1 fog is atmosphere color at zenith.")]
	[TOD_Range(0.0f, 1f)]
	public float HeightBias;

	public float StartDistance;
	public float GlobalDensity = 1f / 1000f;
	public float TransparentDensityMultiplier = 5f;
}