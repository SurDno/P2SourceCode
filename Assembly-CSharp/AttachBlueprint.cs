using System;
using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using Inspectors;

public class AttachBlueprint : MonoBehaviour, IEntityAttachable
{
  [SerializeField]
  private FlowScriptController prefab;
  [Inspected]
  private FlowScriptController controller;

  public void Attach(IEntity owner)
  {
    controller = BlueprintServiceUtility.Start(prefab?.gameObject, owner, (Action) null, owner.GetInfo());
  }

  public void Detach()
  {
    if (!((UnityEngine.Object) controller != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) controller.gameObject);
    controller = null;
  }
}
