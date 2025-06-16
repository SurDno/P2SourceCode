// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.EntityByGameObjectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class EntityByGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.AddValueOutput<IEntity>("Entity", (ValueHandler<IEntity>) (() =>
      {
        GameObject context = this.goInput.value;
        if (!((Object) context != (Object) null))
          return (IEntity) null;
        IEntity entity = EntityUtility.GetEntity(this.goInput.value);
        if (entity == null)
          Debug.LogError((object) ("Entity not found : " + context.name), (Object) context);
        return entity;
      }));
    }
  }
}
