using UnityEngine;

public class LandingButtons : MonoBehaviour
{
  public LandingSpotController _landingSpotController;
  public FlockController _flockController;
  public float hSliderValue = 250f;

  public void OnGUI()
  {
    GUI.Label(new Rect(20f, 20f, 125f, 18f), "Landing Spots: " + _landingSpotController.transform.childCount);
    if (GUI.Button(new Rect(20f, 40f, 125f, 18f), "Scare All"))
      _landingSpotController.ScareAll();
    if (GUI.Button(new Rect(20f, 60f, 125f, 18f), "Land In Reach"))
      _landingSpotController.LandAll();
    if (GUI.Button(new Rect(20f, 80f, 125f, 18f), "Land Instant"))
      StartCoroutine(_landingSpotController.InstantLand(0.01f));
    if (GUI.Button(new Rect(20f, 100f, 125f, 18f), "Destroy"))
      _flockController.DestroyBirds();
    GUI.Label(new Rect(20f, 120f, 125f, 18f), "Bird Amount: " + _flockController._childAmount);
    _flockController._childAmount = (int) GUI.HorizontalSlider(new Rect(20f, 140f, 125f, 18f), _flockController._childAmount, 0.0f, 250f);
  }
}
