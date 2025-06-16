using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;
using System;
using UnityEngine;

[DisallowMultipleComponent]
public class DialogIndication : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  private IEntity owner;
  [Inspected]
  private SpeakingComponent speakingComponent;
  private FocusEffect effect;
  private bool speakingAvailable;
  private bool compassEnabled;
  private bool visible;

  void IEntityAttachable.Attach(IEntity owner) => this.SetOwner(owner);

  void IEntityAttachable.Detach() => this.SetOwner((IEntity) null);

  private void OnEnable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    this.OnEnableChanged(service.IsEnabled);
    service.OnEnableChanged += new Action<bool>(this.OnEnableChanged);
  }

  private void OnDisable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    service.OnEnableChanged -= new Action<bool>(this.OnEnableChanged);
    this.OnEnableChanged(false);
  }

  private void SetOwner(IEntity value)
  {
    if (this.owner == value)
      return;
    this.owner = value;
    this.SetSpeakingComponent(this.owner?.GetComponent<SpeakingComponent>());
  }

  private void SetSpeakingComponent(SpeakingComponent value)
  {
    if (this.speakingComponent == value)
      return;
    if (this.speakingComponent != null)
      this.speakingComponent.OnSpeakAvailableChange -= new Action<bool>(this.SetSpeakingAvailable);
    this.speakingComponent = value;
    if (this.speakingComponent != null)
    {
      this.SetSpeakingAvailable(this.speakingComponent.SpeakAvailable);
      this.speakingComponent.OnSpeakAvailableChange += new Action<bool>(this.SetSpeakingAvailable);
    }
    else
      this.SetSpeakingAvailable(false);
  }

  private void SetSpeakingAvailable(bool value)
  {
    if (this.speakingAvailable == value)
      return;
    this.speakingAvailable = value;
    this.UpdateVisibility();
  }

  private void OnEnableChanged(bool value)
  {
    if (this.compassEnabled == value)
      return;
    this.compassEnabled = value;
    this.UpdateVisibility();
  }

  private void SetVisibility(bool value)
  {
    if (this.visible == value)
      return;
    this.visible = value;
    if ((UnityEngine.Object) this.effect == (UnityEngine.Object) null)
    {
      if (!this.visible)
        return;
      this.effect = this.GetComponent<FocusEffect>();
      if ((UnityEngine.Object) this.effect == (UnityEngine.Object) null)
        this.effect = this.gameObject.AddComponent<FocusEffect>();
    }
    else
      this.effect.enabled = this.visible;
  }

  private void UpdateVisibility()
  {
    this.SetVisibility(this.speakingAvailable && this.compassEnabled);
  }
}
