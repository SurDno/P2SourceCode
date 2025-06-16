using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableParticleEmission : HideableView
  {
    [SerializeField]
    private ParticleSystem particleSystem = (ParticleSystem) null;

    protected override void ApplyVisibility()
    {
      if (!((Object) this.particleSystem != (Object) null))
        return;
      this.particleSystem.emission.enabled = this.Visible;
    }
  }
}
