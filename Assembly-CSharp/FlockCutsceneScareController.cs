using System.Collections;

public class FlockCutsceneScareController : MonoBehaviour
{
  public LandingSpotController landingSpotController;

  private void OnEnable() => this.StartCoroutine(Scare());

  private IEnumerator Scare()
  {
    if ((Object) landingSpotController != (Object) null)
    {
      yield return (object) new WaitForEndOfFrame();
      landingSpotController.ScareAll();
    }
  }
}
