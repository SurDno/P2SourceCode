// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.WaitGameActionNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.Inputs;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;
using System;

#nullable disable
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
