// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.VisibilityEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using UnityEngine;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("Visibility")]
  [Category("Events/Object")]
  [Description("Calls events based on object's render visibility")]
  public class VisibilityEvents : EventNode<Transform>
  {
    private FlowOutput onVisible;
    private FlowOutput onInvisible;

    protected override string[] GetTargetMessageEvents()
    {
      return new string[2]
      {
        "OnBecameVisible",
        "OnBecameInvisible"
      };
    }

    protected override void RegisterPorts()
    {
      this.onVisible = this.AddFlowOutput("Became Visible");
      this.onInvisible = this.AddFlowOutput("Became Invisible");
    }

    private void OnBecameVisible() => this.onVisible.Call();

    private void OnBecameInvisible() => this.onInvisible.Call();
  }
}
