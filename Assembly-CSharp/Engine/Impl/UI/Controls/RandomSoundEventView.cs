using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class RandomSoundEventView : EventView
  {
    [SerializeField]
    private SoundCollection soundCollection;

    public override void Invoke()
    {
      AudioClip clip = this.soundCollection?.GetClip();
      if ((Object) clip == (Object) null)
        return;
      MonoBehaviourInstance<UISounds>.Instance?.PlaySound(clip);
    }
  }
}
