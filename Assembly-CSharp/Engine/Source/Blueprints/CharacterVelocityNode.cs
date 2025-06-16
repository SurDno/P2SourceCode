// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.CharacterVelocityNode
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
  public class CharacterVelocityNode : FlowControlNode
  {
    private ValueInput<CharacterController> characterControllerInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.characterControllerInput = this.AddValueInput<CharacterController>("Character");
      this.AddValueOutput<Vector3>("Velocity", (ValueHandler<Vector3>) (() =>
      {
        CharacterController characterController = this.characterControllerInput.value;
        return (Object) characterController != (Object) null ? characterController.velocity : Vector3.zero;
      }));
    }
  }
}
