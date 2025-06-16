// Decompiled with JetBrains decompiler
// Type: Pathologic.Prototype.LightManagable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.DateTime;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

#nullable disable
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
      this.light = this.GetComponent<Light>();
      if ((bool) (Object) this.BulbObject)
        this.bulbRenderer = this.BulbObject.GetComponent<Renderer>();
      if ((Object) this.light != (Object) null)
      {
        this.enabled = this.light.enabled;
        if (this.DisableShadowsInPlagueIntro)
        {
          VirtualMachineController service = ServiceLocator.GetService<VirtualMachineController>();
          if (service != null && service.ProjectName == "PathologicPlagueIntro")
            this.light.shadows = LightShadows.None;
        }
      }
      this.EnableLight(this.enabled);
    }

    private void Update()
    {
      if (!InstanceByRequest<EngineApplication>.Instance.IsInitialized)
        return;
      this.current = ServiceLocator.GetService<ITimeService>().SolarTime.GetTimesOfDay();
      if (this.current == TimesOfDay.Night)
      {
        if (this.lightEnabled == TimesOfDayUtility.HasValue(this.timesOfDay, TimesOfDay.Night))
          return;
        this.EnableLight(!this.lightEnabled);
      }
      else if (this.current == TimesOfDay.Morning)
      {
        if (this.lightEnabled == TimesOfDayUtility.HasValue(this.timesOfDay, TimesOfDay.Morning))
          return;
        this.EnableLight(!this.lightEnabled);
      }
      else if (this.current == TimesOfDay.Day)
      {
        if (this.lightEnabled == TimesOfDayUtility.HasValue(this.timesOfDay, TimesOfDay.Day))
          return;
        this.EnableLight(!this.lightEnabled);
      }
      else
      {
        if (this.current != TimesOfDay.Evening || this.lightEnabled == TimesOfDayUtility.HasValue(this.timesOfDay, TimesOfDay.Evening))
          return;
        this.EnableLight(!this.lightEnabled);
      }
    }

    public void EnableLight(bool enable)
    {
      if ((Object) this.light != (Object) null)
        this.light.enabled = enable;
      if ((Object) this.AdditionalLight != (Object) null)
        this.AdditionalLight.enabled = enable;
      LightServiceObject component1 = this.GetComponent<LightServiceObject>();
      if ((Object) component1 != (Object) null)
        component1.enabled = enable;
      LightFlicker2 component2 = this.GetComponent<LightFlicker2>();
      if ((Object) component2 != (Object) null)
        component2.enabled = enable;
      if ((Object) this.bulbRenderer != (Object) null)
      {
        if (enable)
          this.bulbRenderer.SetPropertyBlock((MaterialPropertyBlock) null);
        else
          this.bulbRenderer.SetPropertyBlock(LightManagable.MPB);
      }
      this.lightEnabled = enable;
    }

    private static MaterialPropertyBlock MPB
    {
      get
      {
        if (LightManagable.mpb == null)
        {
          LightManagable.mpb = new MaterialPropertyBlock();
          LightManagable.mpb.SetColor("_EmissionColor", Color.black);
        }
        return LightManagable.mpb;
      }
    }
  }
}
