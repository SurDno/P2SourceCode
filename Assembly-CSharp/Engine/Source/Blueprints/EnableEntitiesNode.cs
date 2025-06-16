// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.EnableEntitiesNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System.Collections.Generic;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EnableEntitiesNode : FlowControlNode
  {
    [Port("Entities")]
    private ValueInput<IEnumerable<IEntity>> entitiesInput;
    [Port("Enable")]
    private ValueInput<bool> enableInput;
    [Port("Out")]
    private FlowOutput output;

    [Port("In")]
    private void In()
    {
      IEnumerable<IEntity> entities = this.entitiesInput.value;
      if (entities != null)
      {
        foreach (IEntity entity in entities)
          entity.IsEnabled = this.enableInput.value;
      }
      this.output.Call();
    }
  }
}
