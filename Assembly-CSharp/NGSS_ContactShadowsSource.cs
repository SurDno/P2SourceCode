public class NGSS_ContactShadowsSource : MonoBehaviourInstance<NGSS_ContactShadowsSource>
{
  public Light Light { get; private set; }

  protected override void Awake()
  {
    Light = this.GetComponent<Light>();
    base.Awake();
  }
}
