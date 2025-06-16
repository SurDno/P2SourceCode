using Engine.Common.Components;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class NpcSayInitialFraseNode : FlowControlNode
  {
    private ValueInput<ISpeakingComponent> targetInput;
    private ValueInput<float> minDistance;
    private ValueInput<float> maxDistance;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = AddFlowOutput("Out");
      AddFlowInput("In", () =>
      {
        ISpeakingComponent speakingComponent = targetInput.value;
        if (speakingComponent != null)
        {
          LipSyncComponent component = speakingComponent.Owner.GetComponent<LipSyncComponent>();
          if (component != null && !component.IsPlaying)
            component.Play3D(speakingComponent.InitialPhrases.Random(), minDistance.value, maxDistance.value, true);
        }
        output.Call();
      });
      targetInput = AddValueInput<ISpeakingComponent>("Speaking");
      minDistance = AddValueInput<float>("MinDistance");
      maxDistance = AddValueInput<float>("MaxDistance");
    }
  }
}
