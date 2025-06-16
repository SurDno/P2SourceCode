using UnityEngine;

public class LandingButtons : MonoBehaviour
{
  public LandingSpotController _landingSpotController;
  public FlockController _flockController;
  public float hSliderValue = 250f;

  public void OnGUI()
  {
    GUI.Label(new Rect(20f, 20f, 125f, 18f), "Landing Spots: " + (object) this._landingSpotController.transform.childCount);
    if (GUI.Button(new Rect(20f, 40f, 125f, 18f), "Scare All"))
      this._landingSpotController.ScareAll();
    if (GUI.Button(new Rect(20f, 60f, 125f, 18f), "Land In Reach"))
      this._landingSpotController.LandAll();
    if (GUI.Button(new Rect(20f, 80f, 125f, 18f), "Land Instant"))
      this.StartCoroutine(this._landingSpotController.InstantLand(0.01f));
    if (GUI.Button(new Rect(20f, 100f, 125f, 18f), "Destroy"))
      this._flockController.DestroyBirds();
    GUI.Label(new Rect(20f, 120f, 125f, 18f), "Bird Amount: " + (object) this._flockController._childAmount);
    this._flockController._childAmount = (int) GUI.HorizontalSlider(new Rect(20f, 140f, 125f, 18f), (float) this._flockController._childAmount, 0.0f, 250f);
  }
}
