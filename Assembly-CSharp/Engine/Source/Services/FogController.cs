using System;
using Engine.Common;
using Engine.Impl.Weather.Element;
using UnityEngine;
using UnityStandardAssets.ImageEffects;
using Object = UnityEngine.Object;

namespace Engine.Source.Services;

[RuntimeService(typeof(FogController))]
public class FogController : IInitialisable {
	private GlobalFog engineFog;

	public event Action<float> DensityChangedEvent;

	public void Initialise() {
		engineFog = Object.FindObjectOfType<GlobalFog>();
	}

	public void Terminate() { }

	public void CopyTo(Fog fog) {
		if (engineFog == null)
			return;
		fog.Density = RenderSettings.fogDensity;
		fog.StartDistance = engineFog.startDistance;
		fog.Height = engineFog.height;
	}

	public void CopyFrom(Fog fog) {
		if (engineFog == null)
			return;
		if (RenderSettings.fogDensity != (double)fog.Density) {
			RenderSettings.fogDensity = fog.Density;
			var densityChangedEvent = DensityChangedEvent;
			if (densityChangedEvent != null)
				densityChangedEvent(fog.Density);
		}

		engineFog.height = fog.Height;
		engineFog.startDistance = fog.StartDistance;
		Shader.SetGlobalFloat("_FogStartDistance", fog.StartDistance);
	}
}