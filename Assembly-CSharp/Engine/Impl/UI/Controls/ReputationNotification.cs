// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.ReputationNotification
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Utility;
using Engine.Behaviours.Localization;
using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Common.Components.Parameters;
using Engine.Common.Services;
using Engine.Common.Types;
using Engine.Impl.Services;
using Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay;
using Engine.Source.Audio;
using Engine.Source.Components;
using Engine.Source.Services.Notifications;
using Inspectors;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class ReputationNotification : UIControl, INotification
  {
    [SerializeField]
    private CanvasGroup canvasGroup;
    [SerializeField]
    private Localizer textRegion;
    [SerializeField]
    private Localizer nearRegionText;
    [SerializeField]
    private ProgressView targetReputationView;
    [SerializeField]
    private ProgressView oldReputationView;
    [SerializeField]
    private HideableView downEffect;
    [SerializeField]
    private HideableView upEffect;
    [SerializeField]
    private AudioClip clip;
    [SerializeField]
    private AudioMixerGroup mixer;
    [SerializeField]
    private float time;
    [SerializeField]
    private float fade;
    [SerializeField]
    private float minStepAnimation = 0.25f;
    private IParameterValue<float> reputationParameter;
    private float progress;
    private UIService ui;
    private bool play;
    private float alpha = -1f;

    [Inspected]
    public bool Complete { get; private set; }

    [Inspected]
    public NotificationEnum Type { get; private set; }

    private void OnReputationChange(float newTarget)
    {
      float progress1 = this.targetReputationView.Progress;
      float progress2 = this.oldReputationView.Progress;
      if ((double) Mathf.Sign(newTarget - progress2) != (double) Mathf.Sign(progress1 - progress2))
        this.StartChange(progress1, newTarget);
      else
        this.StartChange(progress2, newTarget);
      this.progress = Mathf.Min(this.progress, Mathf.Min(this.time - this.progress, this.fade));
    }

    private void Update()
    {
      if (this.Complete || !(this.ui.Active is HudWindow))
        return;
      if (!this.play)
      {
        this.Play();
        this.play = true;
      }
      this.progress += Time.deltaTime;
      if ((double) this.progress > (double) this.time)
      {
        this.Complete = true;
        this.reputationParameter.ChangeValueEvent -= new Action<float>(this.OnReputationChange);
      }
      else
        this.SetAlpha(SoundUtility.ComputeFade(this.progress, this.time, this.fade));
    }

    private void Play()
    {
      if ((UnityEngine.Object) this.clip == (UnityEngine.Object) null || (UnityEngine.Object) this.mixer == (UnityEngine.Object) null)
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
      IRegionComponent result1 = (IRegionComponent) null;
      float result2 = 0.0f;
      this.ApplyValue<IRegionComponent>(ref result1, values, 0);
      this.ApplyValue<float>(ref result2, values, 1);
      if (result1 == null)
      {
        Debug.LogError((object) "Notifications : Reputation : No region parameter");
      }
      else
      {
        this.textRegion.Signature = this.GetRegionName(result1);
        foreach (IRegionComponent nearRegion in ServiceLocator.GetService<ISimulation>().Player.GetComponent<PlayerControllerComponent>().GetNearRegions(result1))
        {
          Localizer localizer = UnityEngine.Object.Instantiate<Localizer>(this.nearRegionText, this.nearRegionText.transform.parent, false);
          localizer.Signature = this.GetRegionName(nearRegion);
          localizer.gameObject.SetActive(true);
        }
        this.reputationParameter = result1.Reputation;
        this.reputationParameter.ChangeValueEvent += new Action<float>(this.OnReputationChange);
        this.StartChange(result2, this.reputationParameter.Value);
      }
    }

    private void StartChange(float oldReputation, float newReputation)
    {
      float f = newReputation - oldReputation;
      float num = Mathf.Sign(f);
      if ((double) f * (double) num < (double) this.minStepAnimation)
        oldReputation = newReputation - this.minStepAnimation * num;
      this.oldReputationView.Progress = Mathf.Clamp01(oldReputation);
      this.targetReputationView.Progress = newReputation;
      this.downEffect.Visible = (double) num == -1.0;
      this.upEffect.Visible = (double) num == 1.0;
    }

    private string GetRegionName(IRegionComponent region)
    {
      LocalizationService service = ServiceLocator.GetService<LocalizationService>();
      IMapItemComponent component = region.GetComponent<IMapItemComponent>();
      string regionName = (string) null;
      if (component != null)
      {
        LocalizedText title = component.Title;
        if (title != LocalizedText.Empty)
          regionName = service.GetText(title);
      }
      if (regionName.IsNullOrEmpty())
        regionName = region.Region.ToString();
      return regionName;
    }

    public void Shutdown() => UnityEngine.Object.Destroy((UnityEngine.Object) this.gameObject);

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
