using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class PlayAudioSourceEventView : EventView
  {
    [SerializeField]
    private AudioSource source;

    public override void Invoke() => this.source?.Play();
  }
}
