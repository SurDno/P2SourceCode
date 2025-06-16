// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.RelayValueInput`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Can be used with a RelayOutput of the same (T) type to get the input port value.")]
  [Category("Flow Controllers/Relay")]
  public class RelayValueInput<T> : FlowControlNode
  {
    [Tooltip("The identifier name of the relay")]
    public string identifier = "MyRelayValueName";

    [HideInInspector]
    public ValueInput<T> port { get; private set; }

    public override string name => string.Format("@ {0}", (object) this.identifier);

    protected override void RegisterPorts() => this.port = this.AddValueInput<T>("Value");
  }
}
