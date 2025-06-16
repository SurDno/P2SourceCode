using FlowCanvas.Nodes;
using ParadoxNotion.Design;

namespace Engine.Source.Blueprints
{
  [Category("Engine")]
  public class CameraTransformNode : FlowControlNode
  {
    protected override void RegisterPorts()
    {
      base.RegisterPorts();
      AddValueOutput("Transform", () => GameCamera.Instance.CameraTransform);
    }
  }
}
