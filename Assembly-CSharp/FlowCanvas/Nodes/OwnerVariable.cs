// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.OwnerVariable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Self")]
  [Description("Returns the Owner GameObject")]
  public class OwnerVariable : VariableNode
  {
    public override string name => "<size=20>SELF</size>";

    protected override void RegisterPorts()
    {
      this.AddValueOutput<GameObject>("Value", (ValueHandler<GameObject>) (() => (bool) (Object) this.graphAgent ? this.graphAgent.gameObject : (GameObject) null));
    }

    public override void SetVariable(object o)
    {
    }
  }
}
