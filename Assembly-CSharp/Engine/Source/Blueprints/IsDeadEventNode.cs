// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.IsDeadEventNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class IsDeadEventNode : EventNode<FlowScriptController>
  {
    private FlowOutput deadOutput;
    private FlowOutput resurrectOutput;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged += new Action<bool>(this.OnIsDeadChanged);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<IsDeadListener>().OnIsDeadChanged -= new Action<bool>(this.OnIsDeadChanged);
      base.OnGraphStoped();
    }

    private void OnIsDeadChanged(bool visible)
    {
      if (visible)
        this.deadOutput.Call();
      else
        this.resurrectOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.deadOutput = this.AddFlowOutput("Dead");
      this.resurrectOutput = this.AddFlowOutput("Resurrect");
    }
  }
}
