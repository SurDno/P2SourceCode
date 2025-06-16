using System;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class UiEffectVisibleNode : FlowControlNode
  {
    private ValueInput<bool> visibleInput;
    private ValueInput<float> timeInput;
    private ValueInput<UiEffectType> typeInput;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        UiEffectType type = typeInput.value;
        if (type == UiEffectType.None)
          type = UiEffectType.Fade;
        MonoBehaviourInstance<UiEffectsController>.Instance.SetVisible(visibleInput.value, type, timeInput.value, (Action) (() => output.Call()));
      });
      visibleInput = AddValueInput<bool>("Visible");
      timeInput = AddValueInput<float>("Time");
      typeInput = AddValueInput<UiEffectType>("Type");
    }
  }
}
