using FlowCanvas;
using FlowCanvas.Nodes;
using InputServices;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System;
using UnityEngine;
using UnityEngine.Playables;

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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        this.director = !((UnityEngine.Object) this.director != (UnityEngine.Object) null) ? this.directorInput.value : throw new Exception();
        if ((UnityEngine.Object) this.director == (UnityEngine.Object) null)
        {
          output.Call();
        }
        else
        {
          Action<PlayableDirector> stopped = (Action<PlayableDirector>) null;
          stopped = (Action<PlayableDirector>) (tmp =>
          {
            this.director.stopped -= stopped;
            this.director = (PlayableDirector) null;
            output.Call();
          });
          this.director.stopped += stopped;
          this.director.Play();
        }
      }));
      this.directorInput = this.AddValueInput<PlayableDirector>("Director");
      this.interruptibleInput = this.AddValueInput<bool>("Interruptible");
    }

    public void Update()
    {
      if (!this.interruptibleInput.value || (UnityEngine.Object) this.director == (UnityEngine.Object) null || !Input.GetKeyDown(KeyCode.Escape) && !InputService.Instance.GetButtonDown("B", false))
        return;
      this.director.Stop();
    }
  }
}
