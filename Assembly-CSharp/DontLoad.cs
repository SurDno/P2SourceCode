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
    model = owner.GetComponent<StaticModelComponent>();
    if (model == null)
      return;
    model.NeedLoad = false;
  }

  public void Detach() => model = null;
}
