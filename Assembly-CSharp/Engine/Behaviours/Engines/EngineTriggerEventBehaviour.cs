using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

namespace Engine.Behaviours.Engines
{
  public class EngineTriggerEventBehaviour : MonoBehaviour, IEntityAttachable
  {
    private TriggerComponent component;

    public void Attach(IEntity owner)
    {
      this.component = owner.GetComponent<TriggerComponent>();
      if (this.component == null)
        return;
      this.component.Attach();
    }

    public void Detach()
    {
      if (this.component == null)
        return;
      this.component.Detach();
      this.component = (TriggerComponent) null;
    }

    private void OnTriggerEnter(Collider collider)
    {
      if (this.component == null)
        return;
      this.component.Enter(collider.gameObject);
    }

    private void OnTriggerExit(Collider collider)
    {
      if (this.component == null)
        return;
      this.component.Exit(collider.gameObject);
    }
  }
}
