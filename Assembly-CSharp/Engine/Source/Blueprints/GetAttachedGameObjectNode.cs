// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.GetAttachedGameObjectNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class GetAttachedGameObjectNode : FlowControlNode
  {
    private ValueInput<GameObject> goInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.goInput = this.AddValueInput<GameObject>("GameObject");
      this.AddValueOutput<GameObject>("Attached", (ValueHandler<GameObject>) (() =>
      {
        GameObject context = this.goInput.value;
        if ((Object) context != (Object) null)
        {
          AttachedGameObject component = context.GetComponent<AttachedGameObject>();
          if ((Object) component != (Object) null)
            return component.Attached;
          Debug.LogError((object) ("Attached object not found : " + context.name), (Object) context);
        }
        return (GameObject) null;
      }));
    }
  }
}
