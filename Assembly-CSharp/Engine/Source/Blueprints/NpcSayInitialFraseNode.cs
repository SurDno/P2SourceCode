using Engine.Common.Commons;
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
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ISpeakingComponent speakingComponent = this.targetInput.value;
        if (speakingComponent != null)
        {
          LipSyncComponent component = speakingComponent.Owner.GetComponent<LipSyncComponent>();
          if (component != null && !component.IsPlaying)
            component.Play3D(speakingComponent.InitialPhrases.Random<ILipSyncObject>(), this.minDistance.value, this.maxDistance.value, true);
        }
        output.Call();
      }));
      this.targetInput = this.AddValueInput<ISpeakingComponent>("Speaking");
      this.minDistance = this.AddValueInput<float>("MinDistance");
      this.maxDistance = this.AddValueInput<float>("MaxDistance");
    }
  }
}
