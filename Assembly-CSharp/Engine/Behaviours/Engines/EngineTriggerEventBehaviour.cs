// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Engines.EngineTriggerEventBehaviour
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using UnityEngine;

#nullable disable
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
