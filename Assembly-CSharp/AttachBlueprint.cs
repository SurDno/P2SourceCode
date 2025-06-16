using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using Inspectors;
using System;
using UnityEngine;

public class AttachBlueprint : MonoBehaviour, IEntityAttachable
{
  [SerializeField]
  private FlowScriptController prefab;
  [Inspected]
  private FlowScriptController controller;

  public void Attach(IEntity owner)
  {
    this.controller = BlueprintServiceUtility.Start(this.prefab?.gameObject, owner, (Action) null, owner.GetInfo());
  }

  public void Detach()
  {
    if (!((UnityEngine.Object) this.controller != (UnityEngine.Object) null))
      return;
    UnityEngine.Object.Destroy((UnityEngine.Object) this.controller.gameObject);
    this.controller = (FlowScriptController) null;
  }
}
