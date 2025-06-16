// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.RegionNotification
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Components.Regions;
using Engine.Source.Services.Notifications;
using Inspectors;
using RegionReputation;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class RegionNotification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Localizer textRegion;
    [SerializeField]
    private Localizer textReputation;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
    [SerializeField]
    private RegionReputationNames regionReputationNames;
    private float progress;
    private UIService ui;
    private bool play;
    private float alpha = -1f;

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void Update()
    {
      if (!(this.ui.Active is HudWindow))
        return;
      if (!this.play)
      {
        this.Play();
        this.play = true;
      }
      this.progress += Time.deltaTime;
      if ((double) this.progress > (double) this.time)
        this.Complete = true;
      else
        this.SetAlpha(SoundUtility.ComputeFade(this.progress, this.time, this.fade));
    }

    private void Play()
    {
      if ((Object) this.clip == (Object) null || (Object) this.mixer == (Object) null)
        return;
      SoundUtility.PlayAudioClip2D(this.clip, this.mixer, 1f, 0.0f, context: this.gameObject.GetFullName());
    }

    protected override void Awake()
    {
      base.Awake();
      this.ui = ServiceLocator.GetService<UIService>();
      this.SetAlpha(0.0f);
    }

    public void Initialise(NotificationEnum type, object[] values)
    {
      this.Type = type;
      IRegionComponent result = (IRegionComponent) null;
      this.ApplyValue<IRegionComponent>(ref result, values, 0);
      if (result == null)
      {
        Debug.LogError((object) "Notifications : Region : No region parameter");
      }
      else
      {
        float reputation = result.Reputation.Value;
        this.textRegion.Signature = RegionUtility.GetRegionTitle(result);
        this.textReputation.Signature = this.regionReputationNames.GetReputationName(result.Region, reputation);
      }
    }

    public void Shutdown() => Object.Destroy((Object) this.gameObject);

    private void SetAlpha(float value)
    {
      if ((double) this.alpha == (double) value)
        return;
      this.alpha = value;
      this.canvasGroup.alpha = this.alpha;
    }

    private void ApplyValue<T>(ref T result, object[] values, int index)
    {
      if (index >= values.Length)
        return;
      object obj1 = values[index];
      if (obj1 == null || !(obj1 is T obj2))
        return;
      result = obj2;
    }
  }
}
