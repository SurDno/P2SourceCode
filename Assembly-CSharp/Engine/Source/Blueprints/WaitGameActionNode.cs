using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class WaitGameActionNode : FlowControlNode
  {
    [Port("GameAction")]
    private ValueInput<GameActionType> gameActionInput;
    private FlowOutput output;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      this.output = this.AddFlowOutput("Out");
    }

    public override void OnGraphStarted()
    {
      base.OnGraphStarted();
      ServiceLocator.GetService<GameActionService>().OnGameAction += new Action<GameActionType>(this.WaitGameActionNode_OnGameAction);
    }

    public override void OnGraphStoped()
    {
      ServiceLocator.GetService<GameActionService>().OnGameAction -= new Action<GameActionType>(this.WaitGameActionNode_OnGameAction);
      base.OnGraphStoped();
    }

    private void WaitGameActionNode_OnGameAction(GameActionType type)
    {
      if (type != this.gameActionInput.value || !((UnityEngine.Object) this.graphAgent != (UnityEngine.Object) null))
        return;
      this.output.Call();
    }
  }
}
