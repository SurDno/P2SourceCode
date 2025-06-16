// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ApplicationPauseEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Application Pause")]
  [Category("Events/Application")]
  [Description("Called when the Application is paused or resumed")]
  public class ApplicationPauseEvent : EventNode
  {
    private FlowOutput pause;
    private bool isPause;

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onApplicationPause += new Action<bool>(this.ApplicationPause);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onApplicationPause -= new Action<bool>(this.ApplicationPause);
    }

    private void ApplicationPause(bool isPause)
    {
      this.isPause = isPause;
      this.pause.Call();
    }

    protected override void RegisterPorts()
    {
      this.pause = this.AddFlowOutput("Out");
      this.AddValueOutput<bool>("Is Pause", (ValueHandler<bool>) (() => this.isPause));
    }
  }
}
