using Engine.Source.Audio;
using UnityEngine;

namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class PivotCloud : MonoBehaviour
  {
    [Header("Audio")]
    public AudioSource AudioSource;

    private void Awake() => EnableSound(false);

    public void EnableSound(bool enabled)
    {
      if (!(AudioSource != null))
        return;
      if (enabled)
        AudioSource.PlayAndCheck();
      else
        AudioSource.Stop();
    }

    public void SetSoundMaxDistance(float distance)
    {
      if (!(AudioSource != null))
        return;
      AudioSource.maxDistance = distance;
    }
  }
}
