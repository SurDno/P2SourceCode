using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;

namespace Pathologic.Prototype
{
  public class LightManagable : MonoBehaviour
  {
    [SerializeField]
    private bool DisableShadowsInPlagueIntro;
    [EnumFlag(typeof (TimesOfDay))]
    [SerializeField]
    private TimesOfDay timesOfDay;
    [SerializeField]
    private Light AdditionalLight;
    [SerializeField]
    private GameObject BulbObject;
    [Tooltip("Time offset in minutes to make light turning on/off more random looking")]
    [SerializeField]
    public float TimeOffsetInMinutes;
    [Inspected]
    private TimesOfDay current;
    private Light light;
    private Renderer bulbRenderer;
    private bool lightEnabled;
    private static MaterialPropertyBlock mpb = (MaterialPropertyBlock) null;

    private void Start()
    {
      light = this.GetComponent<Light>();
      if ((bool) (Object) BulbObject)
        bulbRenderer = BulbObject.GetComponent<Renderer>();
      if ((Object) light != (Object) null)
      {
        this.enabled = light.enabled;
        if (DisableShadowsInPlagueIntro)
        {
          VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
          if (service != null && service.ProjectName == "PathologicPlagueIntro")
            light.shadows = LightShadows.None;
        }
      }
      EnableLight(this.enabled);
    }

    private void Update()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsInitialized)
        return;
      current = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
      if (current == TimesOfDay.Night)
      {
        if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Night))
          return;
        EnableLight(!lightEnabled);
      }
      else if (current == TimesOfDay.Morning)
      {
        if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Morning))
          return;
        EnableLight(!lightEnabled);
      }
      else if (current == TimesOfDay.Day)
      {
        if (lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Day))
          return;
        EnableLight(!lightEnabled);
      }
      else
      {
        if (current != TimesOfDay.Evening || lightEnabled == TimesOfDayUtility.HasValue(timesOfDay, TimesOfDay.Evening))
          return;
        EnableLight(!lightEnabled);
      }
    }

    public void EnableLight(bool enable)
    {
      if ((Object) light != (Object) null)
        light.enabled = enable;
      if ((Object) AdditionalLight != (Object) null)
        AdditionalLight.enabled = enable;
      LightServiceObject component1 = this.GetComponent<LightServiceObject>();
      if ((Object) component1 != (Object) null)
        component1.enabled = enable;
      LightFlicker2 component2 = this.GetComponent<LightFlicker2>();
      if ((Object) component2 != (Object) null)
        component2.enabled = enable;
      if ((Object) bulbRenderer != (Object) null)
      {
        if (enable)
          bulbRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
        else
          bulbRenderer.SetPropertyBlock(MPB);
      }
      lightEnabled = enable;
    }

    private static MaterialPropertyBlock MPB
    {
      get
      {
        if (mpb == null)
        {
          mpb = new MaterialPropertyBlock();
          mpb.SetColor("_EmissionColor", Color.black);
        }
        return mpb;
      }
    }
  }
}
