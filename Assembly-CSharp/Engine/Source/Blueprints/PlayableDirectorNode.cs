using System;
using FlowCanvas;
using FlowCanvas.Nodes;
using InputServices;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class PlayableDirectorNode : FlowControlNode, IUpdatable
  {
    private ValueInput<PlayableDirector> directorInput;
    private ValueInput<bool> interruptibleInput;
    private PlayableDirector director;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        director = !((UnityEngine.Object) director != (UnityEngine.Object) null) ? directorInput.value : throw new Exception();
        if ((UnityEngine.Object) director == (UnityEngine.Object) null)
        {
          output.Call();
        }
        else
        {
          Action<PlayableDirector> stopped = (Action<PlayableDirector>) null;
          stopped = (Action<PlayableDirector>) (tmp =>
          {
            director.stopped -= stopped;
            director = (PlayableDirector) null;
            output.Call();
          });
          director.stopped += stopped;
          director.Play();
        }
      });
      directorInput = AddValueInput<PlayableDirector>("Director");
      interruptibleInput = AddValueInput<bool>("Interruptible");
    }

    public void Update()
    {
      if (!interruptibleInput.value || (UnityEngine.Object) director == (UnityEngine.Object) null || !Input.GetKeyDown(KeyCode.Escape) && !InputService.Instance.GetButtonDown("B", false))
        return;
      director.Stop();
    }
  }
}
