// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.CharacterControllerEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Character Controller")]
  [Category("Events/Object")]
  [Description("Called when the Character Controller hits a collider while performing a Move")]
  public class CharacterControllerEvents : EventNode<CharacterController>
  {
    private ControllerColliderHit hitInfo;
    private FlowOutput hit;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[1]{ "OnControllerColliderHit" };
    }

    protected override void RegisterPorts()
    {
      this.hit = this.AddFlowOutput("Collider Hit");
      this.AddValueOutput<GameObject>("Other", (ValueHandler<GameObject>) (() => this.hitInfo.gameObject));
      this.AddValueOutput<Vector3>("Collision Point", (ValueHandler<Vector3>) (() => this.hitInfo.point));
      this.AddValueOutput<ControllerColliderHit>("Collision Info", (ValueHandler<ControllerColliderHit>) (() => this.hitInfo));
    }

    private void OnControllerColliderHit(ControllerColliderHit hitInfo)
    {
      this.hitInfo = hitInfo;
      this.hit.Call();
    }
  }
}
