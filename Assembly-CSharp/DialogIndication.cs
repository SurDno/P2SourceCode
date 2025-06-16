using Engine.Common;
using Engine.Common.Services;
using Engine.Source.Commons;
using Engine.Source.Components;
using Engine.Source.Services;
using Inspectors;

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

  void IEntityAttachable.Attach(IEntity owner) => SetOwner(owner);

  void IEntityAttachable.Detach() => SetOwner(null);

  private void OnEnable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    OnEnableChanged(service.IsEnabled);
    service.OnEnableChanged += OnEnableChanged;
  }

  private void OnDisable()
  {
    QuestCompassService service = ServiceLocator.GetService<QuestCompassService>();
    if (service == null)
      return;
    service.OnEnableChanged -= OnEnableChanged;
    OnEnableChanged(false);
  }

  private void SetOwner(IEntity value)
  {
    if (owner == value)
      return;
    owner = value;
    SetSpeakingComponent(owner?.GetComponent<SpeakingComponent>());
  }

  private void SetSpeakingComponent(SpeakingComponent value)
  {
    if (speakingComponent == value)
      return;
    if (speakingComponent != null)
      speakingComponent.OnSpeakAvailableChange -= SetSpeakingAvailable;
    speakingComponent = value;
    if (speakingComponent != null)
    {
      SetSpeakingAvailable(speakingComponent.SpeakAvailable);
      speakingComponent.OnSpeakAvailableChange += SetSpeakingAvailable;
    }
    else
      SetSpeakingAvailable(false);
  }

  private void SetSpeakingAvailable(bool value)
  {
    if (speakingAvailable == value)
      return;
    speakingAvailable = value;
    UpdateVisibility();
  }

  private void OnEnableChanged(bool value)
  {
    if (compassEnabled == value)
      return;
    compassEnabled = value;
    UpdateVisibility();
  }

  private void SetVisibility(bool value)
  {
    if (visible == value)
      return;
    visible = value;
    if ((UnityEngine.Object) effect == (UnityEngine.Object) null)
    {
      if (!visible)
        return;
      effect = this.GetComponent<FocusEffect>();
      if ((UnityEngine.Object) effect == (UnityEngine.Object) null)
        effect = this.gameObject.AddComponent<FocusEffect>();
    }
    else
      effect.enabled = visible;
  }

  private void UpdateVisibility()
  {
    SetVisibility(speakingAvailable && compassEnabled);
  }
}
