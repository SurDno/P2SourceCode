// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.GameObjectByEntityNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using Engine.Source.Commons;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GameObjectByEntityNode : FlowControlNode
  {
    private ValueInput<IEntity> entityInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.entityInput = this.AddValueInput<IEntity>("Entity");
      this.AddValueOutput<GameObject>("GameObject", (ValueHandler<GameObject>) (() => ((IEntityView) this.entityInput.value)?.GameObject));
    }
  }
}
