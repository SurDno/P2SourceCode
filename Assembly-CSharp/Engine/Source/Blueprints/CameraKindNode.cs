// Decompiled with JetBrains decompiler
// Type: Engine.Source.Blueprints.CameraKindNode
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Services;
using Engine.Source.Services.CameraServices;
using FlowCanvas;
using FlowCanvas.Nodes;
using ParadoxNotion.Design;

#nullable disable
namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraKindNode : FlowControlNode
  {
    private ValueInput<CameraKindEnum> cameraKindValue;

    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      FlowOutput output = this.AddFlowOutput("Out");
      this.AddFlowInput("In", (FlowHandler) (() =>
      {
        ServiceLocator.GetService<CameraService>().Kind = this.cameraKindValue.value;
        output.Call();
      }));
      this.cameraKindValue = this.AddValueInput<CameraKindEnum>("Kind");
    }
  }
}
