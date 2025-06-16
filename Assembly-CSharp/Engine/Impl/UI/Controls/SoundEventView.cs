using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SoundEventView : EventView
  {
    [SerializeField]
    private AudioClip sound;

    public override void Invoke()
    {
      if ((Object) this.sound == (Object) null || !this.gameObject.activeInHierarchy)
        return;
      MonoBehaviourInstance<UISounds>.Instance?.PlaySound(this.sound);
    }
  }
}
