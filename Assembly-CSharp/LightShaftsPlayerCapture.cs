using UnityEngine;

public class LightShaftsPlayerCapture : MonoBehaviour
{
  public float farDistance = 50f;
  public float nearDistance = 5f;
  public float nearUpdateTime = 0.25f;

  private void OnValidate()
  {
    if ((double) this.nearUpdateTime < 0.0)
      this.nearUpdateTime = 0.0f;
    if ((double) this.nearDistance < 0.0)
      this.nearDistance = 0.0f;
    if ((double) this.farDistance < 0.0)
      this.farDistance = 0.0f;
    if ((double) this.nearDistance <= (double) this.farDistance)
      return;
    this.nearDistance = (float) (((double) this.nearDistance + (double) this.farDistance) * 0.5);
    this.farDistance = this.nearDistance;
  }

  private void Update()
  {
    LightShafts.nearDistance = this.nearDistance;
    LightShafts.farDistance = this.farDistance;
    LightShafts.nearUpdateTime = this.nearUpdateTime;
    LightShafts.playerPosition = this.transform.position;
    LightShafts.isPlayerSet = true;
  }

  private void OnDisable() => LightShafts.isPlayerSet = false;
}
