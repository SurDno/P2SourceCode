using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Components;
using Inspectors;
using UnityEngine;

public class DontLoad : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  private StaticModelComponent model;

  public void Attach(IEntity owner)
  {
    this.model = owner.GetComponent<StaticModelComponent>();
    if (this.model == null)
      return;
    this.model.NeedLoad = false;
  }

  public void Detach() => this.model = (StaticModelComponent) null;
}
