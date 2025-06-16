using UnityEngine;

public class NGSS_ContactShadowsSource : MonoBehaviourInstance<NGSS_ContactShadowsSource>
{
  public Light Light { get; private set; }

  protected override void Awake()
  {
    this.Light = this.GetComponent<Light>();
    base.Awake();
  }
}
