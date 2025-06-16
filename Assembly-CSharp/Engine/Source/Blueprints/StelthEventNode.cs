// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.StelthEventNode
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
  public class StelthEventNode : EventNode<FlowScriptController>
  {
    private FlowOutput showOutput;
    private FlowOutput hideOutput;

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged += new Action<bool>(this.OnVisibleChanged);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<StelthEnableListener>().OnVisibleChanged -= new Action<bool>(this.OnVisibleChanged);
      base.OnGraphStoped();
    }

    private void OnVisibleChanged(bool visible)
    {
      if (visible)
        this.showOutput.Call();
      else
        this.hideOutput.Call();
    }

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.showOutput = this.AddFlowOutput("Show");
      this.hideOutput = this.AddFlowOutput("Hide");
    }
  }
}
