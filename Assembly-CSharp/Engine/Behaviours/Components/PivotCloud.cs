using Engine.Source.Audio;
using UnityEngine;

namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class PivotCloud : MonoBehaviour
  {
    [Header("Audio")]
    public AudioSource AudioSource = (AudioSource) null;

    private void Awake() => this.EnableSound(false);

    public void EnableSound(bool enabled)
    {
      if (!((Object) this.AudioSource != (Object) null))
        return;
      if (enabled)
        this.AudioSource.PlayAndCheck();
      else
        this.AudioSource.Stop();
    }

    public void SetSoundMaxDistance(float distance)
    {
      if (!((Object) this.AudioSource != (Object) null))
        return;
      this.AudioSource.maxDistance = distance;
    }
  }
}
