// Decompiled with JetBrains decompiler
// Type: AttachBlueprint
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using Engine.Source.Services;
using FlowCanvas;
using Inspectors;
using System;
using UnityEngine;

#nullable disable
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
