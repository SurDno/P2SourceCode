using UnityEngine;

public class LightShaftsPlayerCapture : MonoBehaviour
{
  public float farDistance = 50f;
  public float nearDistance = 5f;
  public float nearUpdateTime = 0.25f;

  private void OnValidate()
  {
    if (nearUpdateTime < 0.0)
      nearUpdateTime = 0.0f;
    if (nearDistance < 0.0)
      nearDistance = 0.0f;
    if (farDistance < 0.0)
      farDistance = 0.0f;
    if (nearDistance <= (double) farDistance)
      return;
    nearDistance = (float) ((nearDistance + (double) farDistance) * 0.5);
    farDistance = nearDistance;
  }

  private void Update()
  {
    LightShafts.nearDistance = nearDistance;
    LightShafts.farDistance = farDistance;
    LightShafts.nearUpdateTime = nearUpdateTime;
    LightShafts.playerPosition = transform.position;
    LightShafts.isPlayerSet = true;
  }

  private void OnDisable() => LightShafts.isPlayerSet = false;
}
