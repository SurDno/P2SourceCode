// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.DebugEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using NodeCanvas.Framework;
using ParadoxNotion.Design;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Description("Use to debug send a Flow Signal in PlayMode Only")]
  [Category("Events/Other")]
  public class DebugEvent : EventNode, IUpdatable
  {
    protected override void RegisterPorts() => this.AddFlowOutput("Out");

    public void Update()
    {
    }
  }
}
