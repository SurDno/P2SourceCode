using System.Collections;
using UnityEngine;

public class FlockCutsceneScareController : MonoBehaviour
{
  public LandingSpotController landingSpotController;

  private void OnEnable() => this.StartCoroutine(this.Scare());

  private IEnumerator Scare()
  {
    if ((Object) this.landingSpotController != (Object) null)
    {
      yield return (object) new WaitForEndOfFrame();
      this.landingSpotController.ScareAll();
    }
  }
}
