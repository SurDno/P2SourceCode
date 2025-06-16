namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class PivotCloud : MonoBehaviour
  {
    [Header("Audio")]
    public AudioSource AudioSource = (AudioSource) null;

    private void Awake() => EnableSound(false);

    public void EnableSound(bool enabled)
    {
      if (!((Object) AudioSource != (Object) null))
        return;
      if (enabled)
        AudioSource.PlayAndCheck();
      else
        AudioSource.Stop();
    }

    public void SetSoundMaxDistance(float distance)
    {
      if (!((Object) AudioSource != (Object) null))
        return;
      AudioSource.maxDistance = distance;
    }
  }
}
