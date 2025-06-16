using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

internal class DisableEffectsComponent : MonoBehaviour, IEntityAttachable
{
  public void Attach(IEntity owner)
  {
    EffectsComponent component = owner.GetComponent<EffectsComponent>();
    if (component == null)
      return;
    component.Disabled = true;
  }

  public void Detach()
  {
  }
}
