namespace Engine.Impl.UI.Controls
{
  public class HideableParticleEmission : HideableView
  {
    [SerializeField]
    private ParticleSystem particleSystem = (ParticleSystem) null;

    protected override void ApplyVisibility()
    {
      if (!((Object) particleSystem != (Object) null))
        return;
      particleSystem.emission.enabled = Visible;
    }
  }
}
