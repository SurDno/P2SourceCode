using System;
using UnityEngine;

namespace AmplifyBloom;

[Serializable]
public class AmplifyStarlineCache {
	[SerializeField] internal AmplifyPassCache[] Passes;

	public AmplifyStarlineCache() {
		Passes = new AmplifyPassCache[4];
		for (var index = 0; index < 4; ++index)
			Passes[index] = new AmplifyPassCache();
	}

	public void Destroy() {
		for (var index = 0; index < 4; ++index)
			Passes[index].Destroy();
		Passes = null;
	}
}