using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class HideableViewSound : HideableView
  {
    [SerializeField]
    private AudioSource source;

    protected override void ApplyVisibility()
    {
      if (!Application.isPlaying)
        return;
      if (this.Visible)
        this.source?.Play();
      else
        this.source?.Stop();
    }
  }
}
