using Engine.Common;
using Engine.Source.Commons;
using Inspectors;
using UnityEngine;

[DisallowMultipleComponent]
public class EngineGameObject : MonoBehaviour, IEntityAttachable
{
  [Inspected]
  public IEntity Owner { get; private set; }

  public void Attach(IEntity owner) => Owner = owner;

  public void Detach() => Owner = null;
}
