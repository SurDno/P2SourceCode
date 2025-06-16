// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.GetEulerAnglesNode
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
  public class GetEulerAnglesNode : FlowControlNode
  {
    private ValueInput<Transform> transformInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.transformInput = this.AddValueInput<Transform>("Transform");
      this.AddValueOutput<Vector3>("EulerAngles", (ValueHandler<Vector3>) (() =>
      {
        Transform transform = this.transformInput.value;
        return (Object) transform != (Object) null ? transform.eulerAngles : Vector3.zero;
      }));
    }
  }
}
