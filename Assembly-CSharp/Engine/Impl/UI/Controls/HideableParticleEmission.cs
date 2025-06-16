using UnityEngine;

namespace Engine.Impl.UI.Controls;

public class HideableParticleEmission : HideableView {
	[SerializeField] private ParticleSystem particleSystem;

	protected override void ApplyVisibility() {
		if (!(particleSystem != null))
			return;
		var emission = particleSystem.emission;
		emission.enabled = Visible;
	}
}