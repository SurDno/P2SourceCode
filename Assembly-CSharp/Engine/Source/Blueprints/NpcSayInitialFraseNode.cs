// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.NpcSayInitialFraseNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Common.Components;
using Engine.Source.Components;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
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
