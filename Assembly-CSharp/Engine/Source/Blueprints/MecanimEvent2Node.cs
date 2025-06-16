using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

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
      received = AddFlowOutput("Received");
      mecanimEventsInput = AddValueInput<CutsceneMecanimEvents>("Animator");
      eventNameInput = AddValueInput<string>("Name");
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      CutsceneMecanimEvents cutsceneMecanimEvents = mecanimEventsInput.value;
      if (!((UnityEngine.Object) cutsceneMecanimEvents != (UnityEngine.Object) null))
        return;
      cutsceneMecanimEvents.OnEndAnimationEnd += MecanimEvents_OnEndAnimationEnd;
    }

    public override void OnGraphStoped()
    {
      base.OnGraphStoped();
      CutsceneMecanimEvents cutsceneMecanimEvents = mecanimEventsInput.value;
      if (!((UnityEngine.Object) cutsceneMecanimEvents != (UnityEngine.Object) null))
        return;
      cutsceneMecanimEvents.OnEndAnimationEnd -= MecanimEvents_OnEndAnimationEnd;
    }

    private void MecanimEvents_OnEndAnimationEnd(string name)
    {
      if (!(name == eventNameInput.value))
        return;
      Debug.Log((object) ObjectInfoUtility.GetStream().Append(nameof (MecanimEvent2Node)).Append(" , owner : ").Append(graphAgent.name).Append(" , name : ").Append(name));
      received.Call();
    }
  }
}
