// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.MecanimEvent2Node
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;
using UnityEngine;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class MecanimEvent2Node : EventNode<FlowScriptController>
  {
    private ValueInput<CutsceneMecanimEvents> mecanimEventsInput;
    private ValueInput<string> eventNameInput;
    private FlowOutput received;

    protected override void RegisterPorts()
    {
      this.received = this.AddFlowOutput("Received");
      this.mecanimEventsInput = this.AddValueInput<CutsceneMecanimEvents>("Animator");
      this.eventNameInput = this.AddValueInput<string>("Name");
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      CutsceneMecanimEvents cutsceneMecanimEvents = this.mecanimEventsInput.value;
      if (!((UnityEngine.Object) cutsceneMecanimEvents != (UnityEngine.Object) null))
        return;
      cutsceneMecanimEvents.OnEndAnimationEnd += new Action<string>(this.MecanimEvents_OnEndAnimationEnd);
    }

    public override void OnGraphStoped()
    {
      base.OnGraphStoped();
      CutsceneMecanimEvents cutsceneMecanimEvents = this.mecanimEventsInput.value;
      if (!((UnityEngine.Object) cutsceneMecanimEvents != (UnityEngine.Object) null))
        return;
      cutsceneMecanimEvents.OnEndAnimationEnd -= new Action<string>(this.MecanimEvents_OnEndAnimationEnd);
    }

    private void MecanimEvents_OnEndAnimationEnd(string name)
    {
      if (!(name == this.eventNameInput.value))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (MecanimEvent2Node)).Append(" , owner : ").Append(this.graphAgent.name).Append(" , name : ").Append(name));
      this.received.Call();
    }
  }
}
