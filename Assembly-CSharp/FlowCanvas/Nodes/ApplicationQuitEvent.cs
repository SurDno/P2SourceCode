// Decompiled with JetBrains decompiler
// Type: FlowCanvas.Nodes.ApplicationQuitEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using ParadoxNotion.Design;
using ParadoxNotion.Services;
using System;

#nullable disable
namespace FlowCanvas.Nodes
{
  [Name("On Application Quit")]
  [Category("Events/Application")]
  [Description("Called when the Application quit")]
  public class ApplicationQuitEvent : EventNode
  {
    private FlowOutput quit;

    public override void OnGraphStarted()
    {
      BlueprintManager.current.onApplicationQuit += new Action(this.ApplicationQuit);
    }

    public override void OnGraphStoped()
    {
      BlueprintManager.current.onApplicationQuit -= new Action(this.ApplicationQuit);
    }

    private void ApplicationQuit() => this.quit.Call();

    protected override void RegisterPorts() => this.quit = this.AddFlowOutput("Out");
  }
}
