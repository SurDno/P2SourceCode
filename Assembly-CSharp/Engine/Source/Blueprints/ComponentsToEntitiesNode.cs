// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.ComponentsToEntitiesNode
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
  public class ComponentsToEntitiesNode : FlowControlNode
  {
    [Port("Components")]
    private ValueInput<IEnumerable<IComponent>> componentsInput;

    [Port("Entities")]
    private IEnumerable<IEntity> Entities()
    {
      IEnumerable<IComponent> components = this.componentsInput.value;
      if (components != null)
      {
        foreach (IComponent component in components)
          yield return component.Owner;
      }
    }
  }
}
