using UnityEngine;

public class LightFlicker : MonoBehaviour
{
  private float baseIntensity;
  public bool flicker = true;
  public float flickerIntensity = 0.5f;
  private Light lightComp;

  private void Awake()
  {
    this.lightComp = this.gameObject.GetComponent<Light>();
    this.baseIntensity = this.lightComp.intensity;
  }

  private void Update()
  {
    if (!this.flicker)
      return;
    this.lightComp.intensity = Mathf.Lerp(this.baseIntensity - this.flickerIntensity, this.baseIntensity, Mathf.PerlinNoise(Random.Range(0.0f, 1000f), Time.time));
  }
}
